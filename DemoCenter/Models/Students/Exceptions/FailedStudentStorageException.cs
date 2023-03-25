﻿using System;
using Xeptions;

namespace DemoCenter.Models.Students.Exceptions
{
    public class FailedStudentStorageException : Xeption
    {
        public FailedStudentStorageException(Exception innerException)
            : base(message: "Failed student storage error occured, contact support.", innerException)
        { }
    }
}
