using System;
using Xeptions;

namespace DemoCenter.Models.GroupStudents.Exceptions
{
    public class LockedGroupStudentException : Xeption
    {
        public LockedGroupStudentException(Exception innerException)
            : base(message: "GrouStudent is locked, please try again.", innerException)
        { }
    }
}