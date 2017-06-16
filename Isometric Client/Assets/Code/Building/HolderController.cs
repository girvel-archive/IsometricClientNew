using Assets.Code.Common;
using UnityEngine;

namespace Assets.Code.Building
{
    public class HolderController : MonoBehaviour
    {
        private void OnMouseDown()
        {
            if (BuildingsManager.Current.SelectedBuilding != null)
            {
                BuildingsManager.Current.SelectedBuilding.Holder.GetComponent<SpriteRenderer>().sprite =
                    Sprites.Current.UsualPlain;
            }

            var position = GetComponent<IsometricController>().IsometricPosition;

            BuildingsManager.Current.SelectedBuilding = BuildingsManager.Current.Buildings[(int) position.x, (int) position.y];

            GetComponent<SpriteRenderer>().sprite = Sprites.Current.SelectedPlain;
        }

        private void OnMouseEnter()
        {
            if (GetComponent<SpriteRenderer>().sprite == Sprites.Current.UsualPlain)
            {
                GetComponent<SpriteRenderer>().sprite = Sprites.Current.HighlightedPlain;
            }
        }

        private void OnMouseExit()
        {
            if (GetComponent<SpriteRenderer>().sprite == Sprites.Current.HighlightedPlain)
            {
                GetComponent<SpriteRenderer>().sprite = Sprites.Current.UsualPlain;
            }
        }
    }
}
