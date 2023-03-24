using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class SubjectDependencyValidationException : Xeption
    {
        public SubjectDependencyValidationException(Xeption innerException)
            :base (message : "Subject dependency validation error occured, fix the errors and try again.",
                 innerException)
        {}
    }
}
