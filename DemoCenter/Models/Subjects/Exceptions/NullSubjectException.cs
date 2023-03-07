using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class NullSubjectException :Xeption
    {
        public NullSubjectException() :
            base(message : "Subject is null")
        {
            
        }
    }
}
