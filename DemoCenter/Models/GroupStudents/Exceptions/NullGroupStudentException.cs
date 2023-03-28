using Xeptions;

namespace Taarafo.Core.Models.GroupPosts.Exceptions
{
    public class NullGroupStudentException : Xeption
    {
        public NullGroupStudentException()
            : base(message: "Group student is null.")
        { }
    }
}
