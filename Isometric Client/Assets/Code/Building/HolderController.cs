using System;
using Assets.Code.Common;
using Assets.Code.Net;
using Assets.Code.Ui;
using Assets.Code.Ui.Table;
using UnityEngine;

namespace Assets.Code.Building
{
    public class HolderController : MonoBehaviour
    {
        private void OnMouseDown()
        {
            UiManager.Current.SelectBuilding(GetComponent<IsometricController>().IsometricPosition.ToIsometricVector());
        }

        private void OnMouseEnter()
        {
            if (GetComponent<SpriteRenderer>().sprite == Sprites.Current.UsualPlain)
            {
                GetComponent<SpriteRenderer>().sprite = Sprites.Current.HighlightedPlain;
            }

            if (BuildingsManager.Current.SelectedBuilding == null)
            {
                UiManager.Current.ShowInformation(
                    GetComponent<IsometricController>().IsometricPosition.ToIsometricVector());
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
