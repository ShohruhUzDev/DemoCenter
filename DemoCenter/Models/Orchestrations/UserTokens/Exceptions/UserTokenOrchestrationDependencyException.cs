
using Xeptions;

namespace DemoCenter.Models.Orchestrations.Exceptions
{
    public class UserTokenOrchestrationDependencyException : Xeption
    {
        public UserTokenOrchestrationDependencyException(Xeption innerException)
            : base(message: "User token dependency error occurred, contact support.", innerException)
        { }
    }
}
