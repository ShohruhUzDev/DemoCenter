using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Users.Exceptions
{
    public class InvalidUserReferenceException : Xeption
    {
        public InvalidUserReferenceException(Exception innerException)
            : base(message: "Invalid user reference error occured.", innerException)
        { }
    }
}
