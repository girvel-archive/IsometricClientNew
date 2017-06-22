using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Table
{
    public class TableButton : MonoBehaviour
    {
        public Action Click = () => { };

        public Sprite Icon
        {
            get { return GetComponent<Image>().sprite; }
            set { GetComponent<Image>().sprite = value; }
        }



        public void OnClick()
        {
            if (Click != null)
            {
                Click();
            }

            UiManager.Current.RefreshSelectedBuilding();
        }
    }
}