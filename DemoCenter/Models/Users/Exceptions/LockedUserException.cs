﻿using System;
using Xeptions;

namespace DemoCenter.Models.Users.Exceptions
{
    public class LockedUserException : Xeption
    {
        public LockedUserException(Exception innerException)
            : base(message: "User is locked, please try again.", innerException)
        { }
    }
}
