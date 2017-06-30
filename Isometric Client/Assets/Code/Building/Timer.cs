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
                _value = value;
                GetComponent<TextMesh>().text = (_value + TimeSpan.FromSeconds(1)).ToTimerString();
            }
        }

        private TimeSpan _value;



        private void Update()
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