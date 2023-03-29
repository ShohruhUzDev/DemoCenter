using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class GroupStudentDependencyValidationException : Xeption
    {
        public GroupStudentDependencyValidationException(Xeption innerException)
            : base(message: "Group student dependency validation occurred, please try again.",
                  innerException)
        { }
    }
}
