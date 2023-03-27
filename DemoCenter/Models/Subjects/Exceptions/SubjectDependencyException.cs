using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class SubjectDependencyException : Xeption
    {
        public SubjectDependencyException(Xeption innerExceptioin)
            : base(message: "Subject dependency error occured, contact support.", innerExceptioin)
        { }
    }
}
