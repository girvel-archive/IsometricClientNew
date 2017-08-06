using System;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface;
using Assets.Code.Net;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code.Building
{
    public class HolderController : MonoBehaviour
    {
        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var position = 
                GetComponent<IsometricController>().IsometricPosition
                .ToIsometricVector();

            if (GameUi.Current.SelectCell(position))
            {
                BuildingsManager.Current.SelectBuilding(position);
            }
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

            GameUi.Current.ShowPreviousData();
        }
    }
}
