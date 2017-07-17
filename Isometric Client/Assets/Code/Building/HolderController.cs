using System;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface;
using Assets.Code.Net;
using UnityEngine;

namespace Assets.Code.Building
{
    public class HolderController : MonoBehaviour
    {
        private void OnMouseDown()
        {
            var position = 
                GetComponent<IsometricController>().IsometricPosition
                .ToIsometricVector();

            BuildingsManager.Current.SelectBuilding(position);
            GameUi.Current.SelectCell(position);
        }

        private void OnMouseEnter()
        {
            if (GetComponent<SpriteRenderer>().sprite == Sprites.Current.UsualPlain)
            {
                GetComponent<SpriteRenderer>().sprite = Sprites.Current.HighlightedPlain;
            }
            
            if (BuildingsManager.Current.SelectedBuilding == null)
            {
                GameUi.Current.HighlightCell(
                    GetComponent<IsometricController>().IsometricPosition
                    .ToIsometricVector());
            }
        }

        private void OnMouseExit()
        {
            if (GetComponent<SpriteRenderer>().sprite == Sprites.Current.HighlightedPlain)
            {
                GetComponent<SpriteRenderer>().sprite = Sprites.Current.UsualPlain;
            }

            GameUi.Current.ReselectLastCell();
        }
    }
}
