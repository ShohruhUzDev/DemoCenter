using System;
using Xeptions;

namespace Tarteeb.Api.Models.Foundations.Users.Exceptions
{
    public class FailedUserServiceException : Xeption
    {
        public FailedUserServiceException(Exception innerException)
            : base(message: "Failed user service error occured, please contact support", innerException)
        { }
    }
}
