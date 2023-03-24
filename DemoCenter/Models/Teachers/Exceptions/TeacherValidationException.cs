﻿using Xeptions;

namespace DemoCenter.Models.Teachers.Exceptions
{
    public class TeacherValidationException : Xeption
    {
        public TeacherValidationException(Xeption innerException) :
            base(message: "Teacher validation error occured, fix the error and try again.", innerException)
        {

        }
    }
}
