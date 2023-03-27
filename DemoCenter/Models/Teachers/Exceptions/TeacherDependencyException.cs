using System;
using Xeptions;

namespace DemoCenter.Models.Teachers.Exceptions
{
    public class TeacherDependencyException : Xeption
    {
        public TeacherDependencyException(Exception innerException)
            : base(message: "Teacher dependency error occured, contact support.", innerException)
        { }
    }
}
