using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class LockedGroupException : Xeption
    {
        public LockedGroupException(Exception innerException)
            : base(message: "Group is locked, please try again.", innerException)
        { }
    }
}
