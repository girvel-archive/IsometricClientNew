using System;

namespace Assets.Code.Common.Helpers
{
    public static class TimeSpanHelper
    {
        public static string ToTimerString(this TimeSpan timeSpan)
        {
            return (timeSpan.TotalHours >= 1 ? (int) Math.Floor(timeSpan.TotalHours) + ":" : "")
                + string.Format("{0}:{1}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}