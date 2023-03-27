using System;
using Xeptions;

namespace Tarteeb.Api.Models.Foundations.Users.Exceptions
{
    public class NotFoundUserException : Xeption
    {
        public NotFoundUserException(Guid userId)
            : base(message: $"Could not find user with id:{userId}.")
        { }

        public NotFoundUserException()
            : base(message: "Could not find user with given credentials")
        { }
    }
}