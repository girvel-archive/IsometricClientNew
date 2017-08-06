using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Common
{
    public class Sprites : SingletonBehaviour<Sprites>
    {
        public Sprite
            UsualPlain,
            HighlightedPlain,
            SelectedPlain,
            UpgradeIcon,
            IncrementWorkers,
            DecrementWorkers,
            Instruments,
            Agriculture,
            Army,
            ArmyForest,
            ArmyHouse,
            Right,
            Left,
            Move,
            Attack,
            DestroyBuilding,
            FlagPlayer,
            FlagNeutral,
            FlagEnemy;



        private Dictionary<string, Sprite> _armiesForBuildings;


        protected override void Start()
        {
            base.Start();

            _armiesForBuildings = new Dictionary<string, Sprite>
            {
                {"Forest", ArmyForest},
                {"House", ArmyHouse},
            };
        }


        public Sprite GetBuilding(string buildingName)
        {
            return Resources.Load<Sprite>("Buildings/" + buildingName);
        }

        public Sprite GetIcon(string iconName)
        {
            return Resources.Load<Sprite>("Icons/" + iconName);
        }

        public Sprite GetArmySpriteForBuilding(string buildingName)
        {
            return Resources.Load<Sprite>("Armies/" + buildingName) ?? Army;
        }
    }
}