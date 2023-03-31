using Xeptions;

namespace DemoCenter.Models.Foundations.Teachers.Exceptions
{
    public class InvalidTeacherException : Xeption
    {
        public InvalidTeacherException()
            : base(message: "Teacher is invalid.")
        {

        }
    }
}
