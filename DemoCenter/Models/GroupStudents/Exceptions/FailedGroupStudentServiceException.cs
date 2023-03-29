using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class FailedGroupStudentServiceException : Xeption
    {
        public FailedGroupStudentServiceException(Exception innerException)
            : base(message: "Failed group student service occurred, please contact support.", innerException)
        { }
    }
}
