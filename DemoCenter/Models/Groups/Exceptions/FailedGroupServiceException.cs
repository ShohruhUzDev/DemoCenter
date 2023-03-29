using System;
using Xeptions;

namespace DemoCenter.Models.GroupStudents.Exceptions
{
    public class FailedGroupServiceException : Xeption
    {
        public FailedGroupServiceException(Exception innerException)
            : base(message: "Failed group service error occured, please contact support.",
                 innerException)
        { }
    }
}
