using System;
using Assets.Code.Building;
using Assets.Code.Interface.Buttons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Code.Interface.Table
{
    public class TableButton : GameUiButton
    {
        public Action<TableButton> Click = b => { };

        public Sprite Icon
        {
            get { return GetComponent<Image>().sprite; }
            set { GetComponent<Image>().sprite = value; }
        }

        public Text InformationText;


        public override void OnHotkeyPress()
        {
            OnClick();
        }


        public void OnClick()
        {
            GameUi.Current.SelectingTargetMode = false;
            Debug.Log(GameUi.Current.SelectingTargetMode);

            GameUi.Current.Refresh();

            if (Click != null)
            {
                Click(this);
            }
        }

        public void SetMode(bool showHotkey)
        {
            HotkeyText.gameObject.SetActive(showHotkey);
            InformationText.gameObject.SetActive(!showHotkey);
        }
    }
}