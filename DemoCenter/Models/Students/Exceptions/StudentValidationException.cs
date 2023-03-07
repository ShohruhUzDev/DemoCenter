using Xeptions;

namespace DemoCenter.Models.Students.Exceptions
{
    public class StudentValidationException :Xeption
    {
        public StudentValidationException(Xeption innerException):
            base(message: "Student validation errir occured, fix the error and try again.", innerException)
        { }
    }
}
