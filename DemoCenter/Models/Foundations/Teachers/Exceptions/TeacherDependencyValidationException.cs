using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Teachers.Exceptions
{
    public class TeacherDependencyValidationException : Xeption
    {
        public TeacherDependencyValidationException(Exception innerException)
            : base(message: "Teacher dependency validation error occured, fix errors and try again.",
                 innerException)
        { }
    }
}
