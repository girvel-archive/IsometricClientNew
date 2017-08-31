using System;
using System.Collections.Generic;
using Assets.Code.Input;
using Assets.Code.Interface.Panels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Code.Interface.Buttons
{
    public abstract class GameUiButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public abstract void OnHotkeyPress();



        public Text HotkeyText;
            
        public KeyCode Hotkey
        {
            get
            {
                KeyCode result;
                return HotkeyToKeyCode.TryGetValue(HotkeyText.text, out result) 
                    ? result 
                    : (KeyCode) Enum.Parse(typeof (KeyCode), HotkeyText.text);
            }
            set { HotkeyText.text = value.ToString(); }
        }

        public string Description;

        private bool _isMouseOver;



        protected virtual void Start()
        {
            Keyboard.Current.Buttons.Add(this);
        }

        protected virtual void Update()
        {
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
            GameUi.Current.ShowPreviousData();
        }



        private static readonly Dictionary<string, KeyCode> HotkeyToKeyCode = new Dictionary<string, KeyCode>
        {
            {"0", KeyCode.Alpha0},
            {"1", KeyCode.Alpha1},
            {"2", KeyCode.Alpha2},
            {"3", KeyCode.Alpha3},
            {"4", KeyCode.Alpha4},
            {"5", KeyCode.Alpha5},
            {"6", KeyCode.Alpha6},
            {"7", KeyCode.Alpha7},
            {"8", KeyCode.Alpha8},
            {"9", KeyCode.Alpha9},
        };
    }
}