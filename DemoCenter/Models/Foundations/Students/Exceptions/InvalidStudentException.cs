using Xeptions;

namespace DemoCenter.Models.Foundations.Students.Exceptions
{
    public class InvalidStudentException : Xeption
    {
        public InvalidStudentException()
            : base(message: "Student is invalid.")
        {

        }
    }
}
