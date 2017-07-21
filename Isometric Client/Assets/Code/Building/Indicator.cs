using System;
using System.Threading;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Building
{
    public class Indicator : MonoBehaviour
    {
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                GetComponent<TextMesh>().text = _text + (Hunger ? "\nHunger" : "");
            }
        }

        public IIndicatorManager Manager { get; set; }

        public bool Hunger
        {
            get { return _hunger; }
            set
            {
                _hunger = value;
                Text = _text;
            }
        }
        private bool _hunger;
        private string _text;


        private void Update()
        {
            if (Manager != null)
            {
                Manager.Update(this, TimeSpan.FromSeconds(Time.deltaTime));
            }
        }
    }
}