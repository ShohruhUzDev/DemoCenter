using Xeptions;

namespace DemoCenter.Models.Teachers.Exceptions
{
    public class InvalidTeacherException : Xeption
    {
        public InvalidTeacherException() 
            :base(message:"Teacher is invalid.")
        {
            
        }
    }
}
