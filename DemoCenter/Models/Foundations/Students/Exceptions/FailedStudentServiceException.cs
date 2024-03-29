﻿using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Students.Exceptions
{
    public class FailedStudentServiceException : Xeption
    {
        public FailedStudentServiceException(Exception innerException)
            : base(message: "Failed student service error occured, please contact support.",
                 innerException)
        { }
    }
}
