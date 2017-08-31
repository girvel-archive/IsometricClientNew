using System.Collections.Generic;
using System.Linq;
using Assets.Code.Common;
using Assets.Code.Interface;
using Assets.Code.Interface.Buttons;
using UnityEngine;

namespace Assets.Code.Input
{
    public class Keyboard : SingletonBehaviour<Keyboard>
    {
        public List<GameUiButton> Buttons = new List<GameUiButton>();

        public bool ConsoleMode = false;



        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.BackQuote))
            {
                ConsoleMode ^= true;
                Ui.Current.ConsoleForm.SetActive(ConsoleMode);
                return;
            }

            if (ConsoleMode)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
                {
                    Debug.Log("Backspace");
                    AdministrationConsole.Current.RemoveCharacter();
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
                {
                    AdministrationConsole.Current.WriteCommand("\n");
                }
                else
                {
                    AdministrationConsole.Current.WriteCommand(UnityEngine.Input.inputString);
                }
            }
            else
            {
                foreach (var button in Buttons.Where(
                    b => b.isActiveAndEnabled && UnityEngine.Input.GetKeyDown(b.Hotkey)))
                {
                    button.OnHotkeyPress();
                }
            }
        }
    }
}