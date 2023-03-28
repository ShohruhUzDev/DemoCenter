using Xeptions;

namespace Taarafo.Core.Models.GroupStudents.Exceptions
{
    public class InvalidGroupStudentException : Xeption
    {
        public InvalidGroupStudentException()
            : base(message: "Invalid group student. Please correct the errors and try again.")
        { }
    }
}
