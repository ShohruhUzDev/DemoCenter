using System;
using Xeptions;

namespace DemoCenter.Models.Teachers.Exceptions
{
    public class LockedTeacherException : Xeption
    {
        public LockedTeacherException(Exception innerException)
            : base(message: "Teacher is locked, please try again.", innerException)
        { }
    }
}
