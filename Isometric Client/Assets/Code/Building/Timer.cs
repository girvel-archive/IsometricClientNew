using System;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface;
using UnityEngine;

namespace Assets.Code.Building
{
    public class Timer : IIndicatorManager
    {
        public TimeSpan Value { get; set; }

        public bool Infinite { get; set; }



        public Timer(TimeSpan value)
        {
            Value = value;
        }

        public void Update(Indicator indicator, TimeSpan deltaTime)
        {
            Value -= deltaTime;

            if (Value > TimeSpan.Zero)
            {
                indicator.Text =
                    Infinite
                        ? "∞"
                        : (Value + TimeSpan.FromSeconds(1)).ToTimerString();
            }
            else
            {
                End(indicator);
            }
        }

        public void End(Indicator indicator)
        {
            if (GameUi.Current.Mode == UiMode.Management
                || (BuildingsManager.Current.SelectedBuilding != null
                    && BuildingsManager.Current.SelectedBuilding.Indicator == indicator))
            {
                ActionProcessor.Current.AddActionToQueue(
                    () =>
                    {
                        GameUi.Current.Refresh();
                    },
                    TimeSpan.FromSeconds(1));
            }

            indicator.Manager = null;
            indicator.Text = "";
        }
    }
}