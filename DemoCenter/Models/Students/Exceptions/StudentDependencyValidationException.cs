using Xeptions;

namespace DemoCenter.Models.Students.Exceptions
{
    public class StudentDependencyValidationException : Xeption
    {
        public StudentDependencyValidationException(Xeption innerException)
            : base(message: "Student dependency validation error occured , fix errors and try again.",
                 innerException)
        { }
    }
}
