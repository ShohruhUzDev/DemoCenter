﻿using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class SubjectValidationException : Xeption
    {

        public SubjectValidationException(Xeption innerException) :
            base(message: " Group validation error occured, fix the error and try again.", innerException)
        {

        }


    }
}
