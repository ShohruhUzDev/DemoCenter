using Xeptions;

namespace Taarafo.Core.Models.GroupPosts.Exceptions
{
    public class GroupStudentDependencyException : Xeption
    {
        public GroupStudentDependencyException(Xeption innerException)
            : base(message: "Group student dependency validation occurred, please try again.", innerException)
        { }
    }
}
