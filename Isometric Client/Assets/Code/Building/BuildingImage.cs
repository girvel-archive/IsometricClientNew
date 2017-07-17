using Isometric.Core.Vectors;
using UnityEngine;

namespace Assets.Code.Building
{
    public class BuildingImage
    {
        public GameObject Building { get; set; }

        public GameObject Army { get; set; }

        public GameObject Holder { get; set; }

        public Vector Position { get; set; }

        public string Name { get; set; }

        public Indicator Indicator { get; set; }



        public BuildingImage() { }

        public BuildingImage(GameObject building, GameObject holder, Vector position, string name)
        {
            Building = building;
            Position = position;
            Name = name;
            Holder = holder;
        }
    }
}
