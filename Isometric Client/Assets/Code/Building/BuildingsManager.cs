using System;
using Assets.Code.Common;
using Isometric.Core;
using UnityEngine;

namespace Assets.Code.Building
{
    public class BuildingsManager : SingletonBehaviour<BuildingsManager>
    {
        public BuildingImage SelectedBuilding { get; set; }

        public BuildingImage[,] Buildings;



        public void SetBuilding(Vector position, string buildingName)
        {
            if (Buildings != null && Buildings[position.X, position.Y] != null)
            {
                Destroy(Buildings[position.X, position.Y].Building);
            }

            (Buildings[position.X, position.Y].Building = Instantiate(GetPrefab(buildingName)))
                .GetComponent<IsometricController>()
                .IsometricPosition = position.ToVector2();
        }

        public void SetUpgrade(Vector position, string buildingName, TimeSpan time)
        {
            var image = Buildings[position.X, position.Y];

            image.Timer = Instantiate(
                Prefabs.Current.BuildingTimer,
                image.Building.transform.position - new Vector3(0, 0.5f, 0),
                new Quaternion())
                .GetComponent<Timer>();

            image.Timer.Value = time;
            SetBuilding(position, "House");
        }

        public void ShowArea(Area area)
        {
            Buildings = new BuildingImage[area.Buildings.GetLength(0), area.Buildings.GetLength(1)];

            for (var x = 0; x < area.Buildings.GetLength(0); x++)
            {
                for (var y = 0; y < area.Buildings.GetLength(1); y++)
                {
                    Buildings[x, y] = new BuildingImage
                    {
                        Holder = Instantiate(Prefabs.Current.Holder),
                        Position = new Vector(x, y),
                        Name = area.Buildings[x, y].Template.Name
                    };
                    
                    Buildings[x, y].Holder
                        .GetComponent<IsometricController>()
                        .IsometricPosition = new Vector2(x, y);

                    SetBuilding(new Vector(x, y), area.Buildings[x, y].Template.Name);
                }
            }
        }



        private static GameObject GetPrefab(string buildingName)
        {
            switch (buildingName)
            {
                case "Plain":
                    return Prefabs.Current.Plain;

                case "Forest":
                    return Prefabs.Current.Forest;

                case "House":
                    return Prefabs.Current.House;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}