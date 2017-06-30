using System;
using Assets.Code.Building;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Code.Interface.Table
{
    public class TableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<TableButton> Click = b => { };

        public Sprite Icon
        {
            get { return GetComponent<Image>().sprite; }
            set { GetComponent<Image>().sprite = value; }
        }

        public string Description { get; set; }

        public KeyCode Hotkey
        {
            get { return _hotkey; }
            set
            {
                HotkeyText.text = Hotkey.ToString();
                _hotkey = value;
            }
        }
        private KeyCode _hotkey;

        public Text HotkeyText, InformationText;


        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(Hotkey))
            {
                OnClick();
            }
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
            InformationPanel.Current.Text = Description + "\n";
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (BuildingsManager.Current.SelectedBuilding == null)
            {
                InformationPanel.Current.Text = "";
            }
            else
            {
                GameUi.Current.RefreshTable();
            }
        }
    }
}