using System;
using Xeptions;

namespace DemoCenter.Models.Users.Exceptions
{
    public class FailedUserServiceException : Xeption
    {
        public FailedUserServiceException(Exception innerException)
            : base(message: "Failed user service error occured, please contact support", innerException)
        { }
    }
}
