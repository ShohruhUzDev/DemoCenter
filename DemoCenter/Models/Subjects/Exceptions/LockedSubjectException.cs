using System;
using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class LockedSubjectException : Xeption
    {
        public LockedSubjectException(Exception innerException)
            : base(message: "Subject is locked, please try again.", innerException)
        { }
    }
}
