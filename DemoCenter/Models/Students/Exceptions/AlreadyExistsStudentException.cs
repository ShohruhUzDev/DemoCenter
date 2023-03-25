using System;
using Xeptions;

namespace DemoCenter.Models.Students.Exceptions
{
    public class AlreadyExistsStudentException : Xeption
    {
        public AlreadyExistsStudentException(Exception innerException)
            : base(message: "Student already exists.", innerException)
        { }
    }
}
