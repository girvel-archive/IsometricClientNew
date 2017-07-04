using System;
using System.Threading;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Building
{
    public class Timer : MonoBehaviour
    {
        public TimeSpan Value
        {
            get { return _value; }
            set
            {
                _infinite = false;
                _value = value;
                GetComponent<TextMesh>().text = (_value + TimeSpan.FromSeconds(1)).ToTimerString();
            }
        }

        public bool Infinite
        {
            get { return _infinite; }
            set
            {
                _infinite = value;
                if (Infinite)
                { 
                    GetComponent<TextMesh>().text = "∞";
                }
                else
                {
                    Value = Value;
                }
            }
        }

        private TimeSpan _value;
        private bool _infinite;


        private void Update()
        {
            if (!Infinite)
            {
                if (Value > TimeSpan.Zero)
                {
                    Value -= new TimeSpan((long) (Time.deltaTime * TimeSpan.TicksPerSecond));
                }
                else
                {
                    ActionProcessor.Current.AddActionToQueue(() =>
                    {
                        GameUi.Current.RefreshTable();
                    },
                        TimeSpan.FromSeconds(1));
                    Destroy(gameObject);
                }
            }
        }
    }
}