using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Students.Exceptions
{
    public class StudentServiceException : Xeption
    {
        public StudentServiceException(Exception innerException)
            : base(message: "Student service error occured, contact support.", innerException)
        { }
    }
}
