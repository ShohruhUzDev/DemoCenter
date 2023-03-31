using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Users.Exceptions
{
    public partial class AlreadyExistsUserException : Xeption
    {
        public AlreadyExistsUserException(Exception innerException)
            : base(message: "User already exists.", innerException)
        { }
    }
}
