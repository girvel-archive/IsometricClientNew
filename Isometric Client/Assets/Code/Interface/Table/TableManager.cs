using System;
using Assets.Code.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interface.Table
{
    public class TableManager : SingletonBehaviour<TableManager>
    {
        public TableButton[] Buttons = new TableButton[15];


        protected override void Start()
        {
            base.Start();

            for (var i = 0; i < Buttons.Length; i++)
            {
                var obj = Instantiate(Prefabs.Current.TableButton, gameObject.transform);
                var transform = obj.GetComponent<RectTransform>();
                transform.anchorMin 
                    = new Vector2(
                        (4.0f + 36 * (i % 5)) / 184,
                        1 - 36.0f * (i / 5 + 1) / 112);

                transform.anchorMax
                    = new Vector2(
                        36.0f * (i % 5 + 1) / 184,
                        1 - (4.0f + 36 * (i / 5)) / 112);

                transform.anchoredPosition = new Vector2(0, 0);
                    
                transform.localScale = new Vector2(1, 1);

                Buttons[i] = obj.GetComponent<TableButton>();
                obj.SetActive(false);
            }
        }


        public void SetButton(int x, int y, Sprite icon, Action<TableButton> action, string description)
        {
            var b = Buttons[y * 5 + x];

            b.gameObject.SetActive(true);
            b.Icon = icon;
            b.Click = action;
            b.Description = description;

            b.Hotkey = Keys.GetKeyFromCoordinates(x, y);
        }

        public void Clear()
        {
            foreach (var button in Buttons)
            {
                button.Description = "";
                button.SetMode(true);
                button.GetComponent<Button>().interactable = true;
                button.gameObject.SetActive(false);
            }
        }
    }
}