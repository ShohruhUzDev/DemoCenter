using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Subjects.Exceptions
{
    public class FailedSubjectServiceException : Xeption
    {
        public FailedSubjectServiceException(Exception innerException)
            : base(message: "Failed subject service error occured, please contact support.",
                 innerException)
        { }
    }
}
