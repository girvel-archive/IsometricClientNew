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
            Number0,
            Number1,
            Instruments,
            Agriculture,
            Army,
            ArmyForest,
            ArmyHouse,
            Right,
            Left,
            Move,
            Attack,
            DestroyBuilding;



        private Dictionary<string, Sprite> 
            _spritesByNames,
            _armiesForBuildings;


        protected override void Start()
        {
            base.Start();

            _spritesByNames = new Dictionary<string, Sprite>
            {
                {"Орудия труда", Instruments},
                {"Земледелие", Agriculture},
            };

            _armiesForBuildings = new Dictionary<string, Sprite>
            {
                {"Forest", ArmyForest},
                {"House", ArmyHouse},
            };
        }


        public Sprite GetByName(string name)
        {
            return _spritesByNames[name];
        }

        public Sprite GetArmySpriteForBuilding(string buildingName)
        {
            Sprite result;
            return _armiesForBuildings.TryGetValue(buildingName, out result) ? result : Army;
        }
    }
}