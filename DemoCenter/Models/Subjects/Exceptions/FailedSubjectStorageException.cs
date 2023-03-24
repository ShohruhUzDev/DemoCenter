using System;
using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class FailedSubjectStorageException : Xeption
    {
        public FailedSubjectStorageException(Exception innerException) 
            : base (message : "Failed subject storage error occured, contact support", innerException)
        {}
    }
}
