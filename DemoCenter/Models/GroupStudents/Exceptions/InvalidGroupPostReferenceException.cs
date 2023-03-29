
using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class InvalidGroupStudentReferenceException : Xeption
    {
        public InvalidGroupStudentReferenceException(Exception innerException)
            : base(message: "Invalid group post reference error occurred.", innerException)
        { }
    }
}
