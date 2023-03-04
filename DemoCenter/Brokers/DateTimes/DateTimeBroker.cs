using System;

namespace DemoCenter.Brokers.DateTimes
{
    public class DateTimeBroker : IDateTimeBroker
    {
        public DateTime GetCurrenDateTime() =>
            DateTime.UtcNow;
    }
}