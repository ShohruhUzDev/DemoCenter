﻿using System;

namespace DemoCenter.Brokers.DateTimes
{
    public class DateTimeBroker : IDateTimeBroker
    {
        public DateTimeOffset GetCurrentDateTime() =>
            DateTime.UtcNow;
    }
}