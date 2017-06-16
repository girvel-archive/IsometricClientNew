using Isometric.Core;
using UnityEngine;

namespace Assets.Code.Building
{
    public class BuildingImage
    {
        public GameObject Building { get; set; }

        public GameObject Holder { get; set; }

        public Vector Position { get; set; }

        public string Name { get; set; }

        public Timer Timer { get; set; }



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
