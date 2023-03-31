
using Xeptions;

namespace DemoCenter.Models.Processings.Users
{
    public class UserProcessingDependencyException : Xeption
    {
        public UserProcessingDependencyException(Xeption innerException)
            : base(message: "User dependency error occurred, contact support.", innerException)
        { }
    }
}
