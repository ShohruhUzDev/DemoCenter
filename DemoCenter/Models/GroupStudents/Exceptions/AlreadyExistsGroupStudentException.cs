﻿
using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class AlreadyExistsGroupStudentException : Xeption
    {
        public AlreadyExistsGroupStudentException(Exception innerException)
            : base(message: "Group Student with the same id already exists.",
                  innerException)
        { }
    }
}
