using System;
using Assets.Code.Common;
using Assets.Code.Ui;
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
                GetComponent<TextMesh>().text = _value.ToTimerString();
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
                UiManager.Current.RefreshSelectedBuilding();
                Destroy(gameObject);
            }
        }
    }
}