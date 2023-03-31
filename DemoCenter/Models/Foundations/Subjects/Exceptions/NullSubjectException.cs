using Xeptions;

namespace DemoCenter.Models.Foundations.Subjects.Exceptions
{
    public class NullSubjectException : Xeption
    {
        public NullSubjectException() :
            base(message: "Subject is null")
        {

        }
    }
}
