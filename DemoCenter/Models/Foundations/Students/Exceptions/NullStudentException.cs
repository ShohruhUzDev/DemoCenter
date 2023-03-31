using Xeptions;

namespace DemoCenter.Models.Foundations.Students.Exceptions
{
    public class NullStudentException : Xeption
    {
        public NullStudentException() :
            base(message: "Student is null.")
        {

        }
    }
}
