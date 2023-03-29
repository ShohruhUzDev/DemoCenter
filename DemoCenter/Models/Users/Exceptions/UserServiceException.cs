using System;
using Xeptions;

namespace DemoCenter.Models.Users.Exceptions
{
    public class UserServiceException : Xeption
    {
        public UserServiceException(Exception innerException)
            : base(message: "User service error occured, contact support.", innerException)
        { }
    }
}
