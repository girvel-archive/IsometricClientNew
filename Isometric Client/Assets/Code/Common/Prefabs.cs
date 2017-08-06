using System;
using UnityEngine;

namespace Assets.Code.Common
{
    public class Prefabs : SingletonBehaviour<Prefabs>
    {
        public GameObject
            Holder,
            Plain,
            Building,
            BuildingTimer,
            TableButton, 
            Army,
            Flag,
            ManagementRow;
    }
}