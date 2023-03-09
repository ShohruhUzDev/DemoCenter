using Xeptions;

namespace DemoCenter.Models.Students.Exceptions
{
    public class InvalidStudentException : Xeption
    {
        public InvalidStudentException()
            : base(message: "Student is invalid.")
        {

        }
    }
}
