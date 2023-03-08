using System;
using Microsoft.Extensions.Logging;

namespace DemoCenter.Brokers.Loggings
{
    public class LoggingBroker : ILoggingBroker
    {
        private ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger) =>
            this.logger = logger;

        public void LogCritical(Exception exception) =>
            this.logger.LogCritical(message: exception.Message, exception: exception);

        public void LogError(Exception exception) =>
            this.logger.LogError(message: exception.Message, exception: exception);
    }
}