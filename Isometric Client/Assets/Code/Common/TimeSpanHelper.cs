﻿using System;

namespace Assets.Code.Common
{
    public static class TimeSpanHelper
    {
        public static TimeSpan Multiple(this TimeSpan t, float k)
        {
            return TimeSpan.FromTicks((long) (t.Ticks * k));
        }
    }
}