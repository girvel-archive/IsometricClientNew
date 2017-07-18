using Isometric.Core.Vectors;
using UnityEngine;

namespace Assets.Code.Building
{
    public class BuildingImage
    {
        public GameObject Building { get; set; }

        public GameObject Army { get; set; }

        public GameObject Holder { get; set; }

        public SpriteRenderer Flag { get; set; }

        public Vector Position { get; set; }

        public string Name { get; set; }

        public Indicator Indicator { get; set; }
    }
}
