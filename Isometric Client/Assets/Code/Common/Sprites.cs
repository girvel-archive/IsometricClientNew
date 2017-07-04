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
            Agriculture;



        private Dictionary<string, Sprite> _spritesByNames;


        protected override void Start()
        {
            base.Start();
            _spritesByNames = new Dictionary<string, Sprite>
            {
                {"Орудия труда", Instruments},
                {"Земледелие", Agriculture},
            };
        }


        public Sprite GetResearchSprite(string name)
        {
            return _spritesByNames[name];
        }
    }
}