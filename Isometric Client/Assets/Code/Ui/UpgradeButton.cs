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

            if (NetManager.Current.TryUpgrade("House", position.ToIsometricVector()))
            {
                BuildingsManager.Current.SetBuilding(position, "House");
            }
        }
    }
}