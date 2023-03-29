using Xeptions;

namespace DemoCenter.Models.GroupStudents.Exceptions
{
    public class GroupDependencyValidationException : Xeption
    {
        public GroupDependencyValidationException(Xeption innerException)
            : base(message: "Group dependency validation error occured, fix the errors and try again."
                 , innerException)
        { }
    }
}
