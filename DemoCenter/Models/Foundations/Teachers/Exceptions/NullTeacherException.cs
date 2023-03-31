using Xeptions;

namespace DemoCenter.Models.Foundations.Teachers.Exceptions
{
    public class NullTeacherException : Xeption
    {
        public NullTeacherException() :
            base(message: "Teacher is null.")
        {

        }
    }
}
