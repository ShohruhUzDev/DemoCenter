
using Xeptions;

namespace DemoCenter.Models.Orchestrations.Exceptions
{
    public class InvalidUserCredentialOrchestrationException : Xeption
    {
        public InvalidUserCredentialOrchestrationException()
            : base(message: "Credential missing. Fix the error and try again.")
        { }
    }
}
