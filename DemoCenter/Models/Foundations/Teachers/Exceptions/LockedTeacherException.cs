using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Teachers.Exceptions
{
    public class LockedTeacherException : Xeption
    {
        public LockedTeacherException(Exception innerException)
            : base(message: "Teacher is locked, please try again.", innerException)
        { }
    }
}
