using System;
using Assets.Code.Building;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Code.Interface.Table
{
    public class TableButton : HotkeyButton, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<TableButton> Click = b => { };

        public Sprite Icon
        {
            get { return GetComponent<Image>().sprite; }
            set { GetComponent<Image>().sprite = value; }
        }

        public string Description { get; set; }

        public Text InformationText;

        private bool _isMouseOver;


        protected override void Update()
        {
            base.Update();

            if (_isMouseOver)
            {
                InformationPanel.Current.Text = Description + "\n";
            }
        }

        protected override void OnHotkeyPress()
        {
            OnClick();
        }


        public void OnClick()
        {
            if (Click != null)
            {
                Click(this);
            }

            GameUi.Current.RefreshTable();
        }
        
        public void OnPointerEnter(PointerEventData data)
        {
            _isMouseOver = true;
        }

        public void OnPointerExit(PointerEventData data)
        {
            _isMouseOver = false;
            if (BuildingsManager.Current.SelectedBuilding == null)
            {
                InformationPanel.Current.Text = "";
            }
            else
            {
                GameUi.Current.RefreshTable();
            }
        }

        public void SetMode(bool showHotkey)
        {
            HotkeyText.gameObject.SetActive(showHotkey);
            InformationText.gameObject.SetActive(!showHotkey);
        }
    }
}