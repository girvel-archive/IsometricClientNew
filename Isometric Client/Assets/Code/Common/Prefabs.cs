using System;
using UnityEngine;

namespace Assets.Code.Common
{
    public class Prefabs : SingletonBehaviour<Prefabs>
    {
        public GameObject
            Holder,
            Plain,
            Forest,
            House,
            BuildingTimer,
            TableButton, 
            Army;



        public GameObject GetPrefab(string serverName)
        {
            switch (serverName)
            {
                case "Plain":
                    return Plain;

                case "Forest":
                    return Forest;

                case "House":
                    return House;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}