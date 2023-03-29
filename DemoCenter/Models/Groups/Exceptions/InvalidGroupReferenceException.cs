using System;
using Xeptions;

namespace DemoCenter.Models.GroupStudents.Exceptions
{
    public class InvalidGroupReferenceException : Xeption
    {
        public InvalidGroupReferenceException(Exception innerException)
                : base(message: "Invalid group reference error occured.", innerException)
        { }
    }
}
