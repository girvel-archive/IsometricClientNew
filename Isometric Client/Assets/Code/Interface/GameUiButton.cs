using System;
using Assets.Code.Building;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Code.Interface
{
    public abstract class GameUiButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected abstract void OnHotkeyPress();



        public Text HotkeyText;
            
        public KeyCode Hotkey
        {
            get { return (KeyCode) Enum.Parse(typeof (KeyCode), HotkeyText.text); }
            set { HotkeyText.text = value.ToString(); }
        }

        public string Description;

        private bool _isMouseOver;



        protected virtual void Update()
        {
            if (UnityEngine.Input.GetKeyDown(Hotkey))
            {
                OnHotkeyPress();
            }

            if (_isMouseOver)
            {
                InformationPanel.Current.Text = Description;
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            _isMouseOver = true;
        }

        public void OnPointerExit(PointerEventData data)
        {
            _isMouseOver = false;
            GameUi.Current.ReselectLastCell();
        }
    }
}