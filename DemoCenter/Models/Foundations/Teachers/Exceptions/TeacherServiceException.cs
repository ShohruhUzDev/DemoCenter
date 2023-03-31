using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Teachers.Exceptions
{
    public class TeacherServiceException : Xeption
    {
        public TeacherServiceException(Exception innerException)
            : base(message: " Teacher service error occured, contact support.", innerException)
        { }
    }
}
