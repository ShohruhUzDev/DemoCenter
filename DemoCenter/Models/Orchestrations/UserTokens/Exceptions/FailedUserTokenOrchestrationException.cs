
using System;
using Xeptions;

namespace DemoCenter.Models.Orchestrations.Exceptions
{
    public class FailedUserTokenOrchestrationException: Xeption
    {
        public FailedUserTokenOrchestrationException(Exception innerException)
            : base(message: "Failed user token service error occurred, contact support.", innerException)
        { }
    }
}
