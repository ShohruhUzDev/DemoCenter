using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Teachers.Exceptions
{
    public class AlreadyExistTeacherException : Xeption
    {
        public AlreadyExistTeacherException(Exception innerException)
            : base(message: "Teacher already exists.", innerException)
        { }
    }
}
