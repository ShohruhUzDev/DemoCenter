using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class InvalidSubjectException : Xeption
    {
        public InvalidSubjectException() :
            base(message: "Subject is invalid.")
        {

        }
    }
}
