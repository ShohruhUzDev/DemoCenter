
using System;
using Xeptions;

namespace Taarafo.Core.Models.GroupStudents.Exceptions
{
    public class InvalidGroupStudentReferenceException : Xeption
    {
        public InvalidGroupStudentReferenceException(Exception innerException)
            : base(message: "Invalid group post reference error occurred.", innerException)
        { }
    }
}
