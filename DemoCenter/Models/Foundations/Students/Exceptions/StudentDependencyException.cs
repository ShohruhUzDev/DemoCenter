using Xeptions;

namespace DemoCenter.Models.Foundations.Students.Exceptions
{
    public class StudentDependencyException : Xeption
    {
        public StudentDependencyException(Xeption innerException)
            : base(message: "Student dependency error occured, contact support.", innerException)
        { }
    }
}
