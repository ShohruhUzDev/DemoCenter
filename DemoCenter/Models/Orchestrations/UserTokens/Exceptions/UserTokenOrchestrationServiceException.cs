
using Xeptions;

namespace DemoCenter.Models.Orchestrations.Exceptions
{
    public class UserTokenOrchestrationServiceException : Xeption
    {
        public UserTokenOrchestrationServiceException(Xeption innerException)
            : base(message: "User token service error occurred, contact support.", innerException)
        { }
    }
}
