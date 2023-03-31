using Xeptions;

namespace DemoCenter.Models.Foundations.Subjects.Exceptions
{
    public class InvalidSubjectException : Xeption
    {
        public InvalidSubjectException() :
            base(message: "Subject is invalid.")
        {

        }
    }
}
