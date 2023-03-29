using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class FailedGroupStorageException : Xeption
    {
        public FailedGroupStorageException(Exception innerException)
            : base(message: "Failed group storage error occured, contact support.", innerException)
        { }
    }
}
