using Xeptions;

namespace DemoCenter.Models.Foundations.Subjects.Exceptions
{
    public class SubjectDependencyValidationException : Xeption
    {
        public SubjectDependencyValidationException(Xeption innerException)
            : base(message: "Subject dependency validation error occured, fix the errors and try again.",
                 innerException)
        { }
    }
}
