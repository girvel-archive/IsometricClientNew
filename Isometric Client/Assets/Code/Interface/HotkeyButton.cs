using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interface
{
    public abstract class HotkeyButton : MonoBehaviour
    {
        protected abstract void OnHotkeyPress();



        public Text HotkeyText;
            
        public KeyCode Hotkey
        {
            get { return (KeyCode) Enum.Parse(typeof (KeyCode), HotkeyText.text); }
            set { HotkeyText.text = value.ToString(); }
        }



        protected virtual void Update()
        {
            if (UnityEngine.Input.GetKeyDown(Hotkey))
            {
                OnHotkeyPress();
            }
        }
    }
}