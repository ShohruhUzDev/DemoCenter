using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Teachers.Exceptions
{
    public class FailedTeacherStorageException : Xeption
    {
        public FailedTeacherStorageException(Exception innerException)
            : base(message: "Failed group storage error occured, contact support.",
                  innerException)
        { }
    }
}
