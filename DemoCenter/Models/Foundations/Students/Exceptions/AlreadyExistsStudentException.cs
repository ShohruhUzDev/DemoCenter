using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Students.Exceptions
{
    public class AlreadyExistsStudentException : Xeption
    {
        public AlreadyExistsStudentException(Exception innerException)
            : base(message: "Student already exists.", innerException)
        { }
    }
}
