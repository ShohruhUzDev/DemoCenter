using Xeptions;

namespace DemoCenter.Models.Foundations.GroupStudents.Exceptions
{
    public class GroupStudentDependencyException : Xeption
    {
        public GroupStudentDependencyException(Xeption innerException)
            : base(message: "Group student dependency validation occurred, please try again.", innerException)
        { }
    }
}
