using System;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Net;
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
            if (Buildings == null)
            {
                throw new NullReferenceException("Call ShowArea() before SetBuilding()");
            }

            if (Buildings[position.X, position.Y].Building != null)
            {
                Destroy(Buildings[position.X, position.Y].Building);
            }

            (Buildings[position.X, position.Y].Building = Instantiate(Prefabs.Current.GetPrefab(buildingName)))
                .GetComponent<IsometricController>()
                .IsometricPosition = position.ToVector2();
        }

        public void SetUpgrade(Vector position, string buildingName, TimeSpan time)
        {
            var image = Buildings[position.X, position.Y];

            SetBuilding(position, buildingName);

            image.Timer = NewTimer(image.Building.transform, time);
        }

        public void ShowArea(NetManager.MainBuildingInfo[,] buildings)
        {
            Buildings = new BuildingImage[buildings.GetLength(0), buildings.GetLength(1)];

            for (var x = 0; x < buildings.GetLength(0); x++)
            {
                for (var y = 0; y < buildings.GetLength(1); y++)
                {
                    var b = buildings[x, y];

                    Buildings[x, y] = new BuildingImage
                    {
                        Holder = Instantiate(Prefabs.Current.Holder),
                        Position = new Vector(x, y),
                        Name = b.Name,
                    };

                    SetBuilding(new Vector(x, y), b.Name);

                    Buildings[x, y].Timer = b.BuildingTime > TimeSpan.Zero
                        ? NewTimer(
                            Buildings[x, y].Building.transform,
                            b.BuildingTime)
                        : null;
                    
                    Buildings[x, y].Holder
                        .GetComponent<IsometricController>()
                        .IsometricPosition = new Vector2(x, y);
                }
            }
        }



        private static Timer NewTimer(Transform parentBuildingTransform, TimeSpan time)
        {
            var result = Instantiate(
                Prefabs.Current.BuildingTimer,
                new Vector3(0, -0.5f, -1),
                new Quaternion(),
                parentBuildingTransform)
                .GetComponent<Timer>();

            result.Value = time;
            return result;
        }
    }
}