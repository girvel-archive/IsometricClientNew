using System.Text;
using Assets.Code.Common;
using Isometric.Core;
using UnityEngine;

namespace Assets.Code.Building
{
    public class BuildingsManager : SingletonBehaviour<BuildingsManager>
    {
        public BuildingImage SelectedBuilding { get; set; }

        public GameObject[,] Buildings;



        public void SetBuilding(Vector2 position, string name)
        {
            if (Buildings != null && Buildings[(int) position.x, (int)position.y] != null)
            {
                Destroy(Buildings[(int) position.x, (int)position.y]);
            }

            (Buildings[(int) position.x, (int) position.y] =
                (GameObject)
                    Instantiate(
                        UnityEngine.Resources.Load<Object>(
                            GetResourceName(name))))
                .GetComponent<IsometricController>()
                .IsometricPosition = position;
        }

        public void ShowArea(Area area)
        {
            Buildings = new GameObject[area.Buildings.GetLength(0), area.Buildings.GetLength(1)];

            for (var x = 0; x < area.Buildings.GetLength(0); x++)
            {
                for (var y = 0; y < area.Buildings.GetLength(1); y++)
                {
                    ((GameObject) Instantiate(UnityEngine.Resources.Load<Object>("Plain")))
                        .GetComponent<IsometricController>()
                        .IsometricPosition = new Vector2(x, y);

                    SetBuilding(new Vector2(x, y), area.Buildings[x, y].Template.Name);
                }
            }
        }

        private string GetResourceName(string buildingName)
        {
            switch (buildingName)
            {
                default:
                    return buildingName;

                case "House":
                    return "House - wood underground";
            }
        }
    }
}