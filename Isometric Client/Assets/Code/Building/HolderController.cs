using UnityEngine;

namespace Assets.Code.Building
{
    public class HolderController : MonoBehaviour
    {
        private void OnMouseDown()
        {
            var position = GetComponent<IsometricController>().IsometricPosition;

            BuildingsManager.Current.SelectedBuilding = new BuildingImage
            {
                GameObject = BuildingsManager.Current.Buildings[(int)position.x, (int)position.y],
                Position = position,
            };
        }
    }
}
