using System;
using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class SubjectServiceException : Xeption
    {
        public SubjectServiceException(Exception innerException)
            : base(message: "Subject service error occured, contact support.", innerException)
        { }
    }
}
