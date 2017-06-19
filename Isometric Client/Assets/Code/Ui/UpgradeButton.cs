using System;
using Assets.Code.Building;
using Assets.Code.Common;
using UnityEngine;

namespace Assets.Code.Ui
{
    public class UpgradeButton : MonoBehaviour
    {
        public void OnClick()
        {
            var position = BuildingsManager.Current.SelectedBuilding.Position;

            TimeSpan upgradeTime;
            if (NetManager.Current.TryUpgrade("House", position, out upgradeTime))
            {
                Debug.Log(upgradeTime);
                BuildingsManager.Current.SetUpgrade(position, "House", upgradeTime);
            }
        }
    }
}