using Xeptions;

namespace Tarteeb.Api.Models.Foundations.Users.Exceptions
{
    public class UserDependencyValidationException : Xeption
    {
        public UserDependencyValidationException(Xeption innerException)
            : base(message: "User dependency validation error occurred, fix the error and try again ", innerException)
        { }
    }
}
