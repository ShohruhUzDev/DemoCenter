using Xeptions;

namespace DemoCenter.Models.Teachers.Exceptions
{
    public class NullTeacherException : Xeption
    {
        public NullTeacherException():
            base(message: "Teacher is null.")
        {
            
        }
    }
}
