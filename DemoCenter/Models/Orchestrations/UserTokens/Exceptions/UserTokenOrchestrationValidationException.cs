
using Xeptions;

namespace DemoCenter.Models.Orchestrations.Exceptions
{
    public class UserTokenOrchestrationValidationException : Xeption
    {
        public UserTokenOrchestrationValidationException(Xeption innerException)
            : base(message: "User token validation error occurred, fix the errors and try again.", innerException)
        { }
    }
}
