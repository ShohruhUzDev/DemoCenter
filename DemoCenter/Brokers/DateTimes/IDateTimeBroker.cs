using System;

namespace DemoCenter.Brokers.DateTimes
{
    public interface IDateTimeBroker
    {
        public DateTimeOffset GetCurrentDateTime();
    }
}
