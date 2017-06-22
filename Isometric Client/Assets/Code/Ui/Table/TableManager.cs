using System;
using Assets.Code.Common;
using Isometric.Core;
using UnityEngine;

namespace Assets.Code.Ui.Table
{
    public class TableManager : SingletonBehaviour<TableManager>
    {
        public TableButton[] Buttons = new TableButton[15];


        protected override void Start()
        {
            base.Start();

            foreach (var b in Buttons)
            {
                b.gameObject.SetActive(false);
            }
        }


        public void SetButton(int x, int y, Sprite icon, Action action)
        {
            var b = Buttons[y * 5 + x];

            b.gameObject.SetActive(true);
            b.Icon = icon;
            b.Click = action;
        }

        public void Clear()
        {
            foreach (var button in Buttons)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}