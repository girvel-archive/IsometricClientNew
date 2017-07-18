using System;
using System.Linq;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface;
using Assets.Code.Net;
using Isometric.Core.Vectors;
using Isometric.Dtos;
using UnityEngine;

namespace Assets.Code.Building
{
    public class BuildingsManager : SingletonBehaviour<BuildingsManager>
    {
        public BuildingImage this[Vector position]
        {
            get { return Buildings[position.X, position.Y]; }
            set { Buildings[position.X, position.Y] = value; }
        }



        public BuildingImage SelectedBuilding { get; set; }

        public BuildingImage[,] Buildings;



        public void SelectBuilding(Vector position)
        {
            ClearBuildingSelection();

            SelectedBuilding = Buildings[position.X, position.Y];
            
            SelectedBuilding.Holder.GetComponent<SpriteRenderer>().sprite
                = Sprites.Current.SelectedPlain;
        }

        public void ClearBuildingSelection()
        {
            if (SelectedBuilding != null)
            {
                SelectedBuilding.Holder.GetComponent<SpriteRenderer>().sprite
                    = Sprites.Current.UsualPlain;
                SelectedBuilding = null;
            }
        }

        public void SetBuilding(Vector position, string buildingName, string ownerName)
        {
            if (Buildings == null)
            {
                throw new NullReferenceException("Call ShowArea() before SetBuilding()");
            }

            var image = Buildings[position.X, position.Y];

            var oldBuilding = image.Building;

            image.Building 
                = Instantiate(
                    Prefabs.Current.GetPrefab(buildingName), 
                    GameObjects.Current.BuildingsContainer.transform);
            
            image.Building
                .GetComponent<IsometricController>()
                .IsometricPosition = position.ToVector2();

            RefreshFlag(buildingName, ownerName, image.Flag);

            if (oldBuilding != null)
            {
                Destroy(oldBuilding);

                foreach (var child in oldBuilding.GetComponentsInChildren<Transform>())
                {
                    child.SetParent(image.Building.transform, false);
                }

                if (image.Army != null)
                {
                    image.Army.GetComponent<SpriteRenderer>().sprite =
                        Sprites.Current.GetArmySpriteForBuilding(buildingName);
                }
            }
        }

        public void SetUpgrade(Vector position, string buildingName, string ownerName, TimeSpan time)
        {
            var image = Buildings[position.X, position.Y];

            SetBuilding(position, buildingName, ownerName);

            SetTimer(position, time);
            image.Name = buildingName;
        }

        public void SetArmy(Vector position)
        {
            var image = this[position];

            if (image.Army != null)
            {
                return;
            }

            image.Army = Instantiate(
                Prefabs.Current.Army,
                image.Building.transform.position,
                new Quaternion(),
                image.Building.transform);

            Debug.Log(image.Name);

            image.Army.GetComponent<SpriteRenderer>().sprite =
                Sprites.Current.GetArmySpriteForBuilding(image.Name);
        }

        public void RemoveArmy(Vector position)
        {
            Debug.Log(this[position].Army.name);
            Destroy(this[position].Army);
            this[position].Army = null;
        }

        public void ShowArea(BuildingAreaDto[,] buildings)
        {
            foreach (
                var childTransform 
                in GameObjects.Current.BuildingsContainer.transform
                    .GetComponentsInChildren<Transform>()
                    .Where(c => c.gameObject != GameObjects.Current.BuildingsContainer))
            {
                Destroy(childTransform.gameObject);
            }

            Buildings = new BuildingImage[buildings.GetLength(0), buildings.GetLength(1)];

            for (var x = 0; x < buildings.GetLength(0); x++)
            {
                for (var y = 0; y < buildings.GetLength(1); y++)
                {
                    var b = buildings[x, y];

                    var image = Buildings[x, y] = new BuildingImage
                    {
                        Holder = Instantiate(
                            Prefabs.Current.Holder,
                            new Vector3(0, 0, -1),
                            new Quaternion(),
                            GameObjects.Current.BuildingsContainer.transform),
                        Position = new Vector(x, y),
                        Name = b.Name,
                    };

                    image.Flag = Instantiate(
                        Prefabs.Current.Flag,
                        Vector3.zero,
                        new Quaternion())
                        .GetComponent<SpriteRenderer>();

                    SetBuilding(new Vector(x, y), b.Name, b.OwnerName);

                    image.Flag.transform.SetParent(image.Building.transform, false);

                    image.Indicator =
                        Instantiate(
                            Prefabs.Current.BuildingTimer,
                            image.Building.transform.position + new Vector3(0, -0.75f, -1),
                            new Quaternion(),
                            image.Building.transform)
                            .GetComponent<Indicator>();

                    if (b.BuildingTime > TimeSpan.Zero)
                    {
                        image.Indicator.Manager = new Timer(b.BuildingTime);
                    }

                    image.Indicator.Hunger = b.ArePeopleHungry;

                    if (b.IsThereArmy)
                    {
                        SetArmy(new Vector(x, y));
                    }
                    
                    image.Holder
                        .GetComponent<IsometricController>()
                        .IsometricPosition = new Vector2(x, y);
                }
            }
        }

        public void SetTimer(Vector position, TimeSpan time)
        {
            if (this[position].Indicator.Manager != null)
            {
                this[position].Indicator.Manager.End(this[position].Indicator);
            }

            this[position].Indicator.enabled = true;
            Debug.Log(time);
            this[position].Indicator.Manager = new Timer(time);
        }



        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                ClearBuildingSelection();
                GameUi.Current.SelectingTargetMode = false;
                GameUi.Current.Clear();
            }
        }



        private readonly string[] _buildingsWithoutFlag =
        {
            "Plain",
            "Forest",
        };

        private void RefreshFlag(string buildingName, string ownerName, SpriteRenderer flag)
        {
            Sprite flagSprite = null;
            if (!_buildingsWithoutFlag.Contains(buildingName))
            {
                if (ownerName == NetManager.Current.LastLogin)
                {
                    flagSprite = Sprites.Current.FlagPlayer;
                }
                else if (ownerName == "no owner")
                {
                    flagSprite = Sprites.Current.FlagNeutral;
                }
                else
                {
                    flagSprite = Sprites.Current.FlagEnemy;
                }
            }
            
            flag.sprite = flagSprite;
        }
    }
}