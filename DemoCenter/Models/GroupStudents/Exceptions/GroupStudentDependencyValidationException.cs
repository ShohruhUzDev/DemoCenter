using Xeptions;

namespace Taarafo.Core.Models.GroupPosts.Exceptions
{
    public class GroupStudentDependencyValidationException : Xeption
    {
        public GroupStudentDependencyValidationException(Xeption innerException)
            : base(message: "Group student dependency validation occurred, please try again.",
                  innerException)
        { }
    }
}
