using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Teachers.Exceptions
{
    public class FailedTeacherServiceException : Xeption
    {
        public FailedTeacherServiceException(Exception innerException)
            : base(message: "Failed teacher service error occured, please contact support.",
                 innerException)
        { }
    }
}
