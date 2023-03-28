using Xeptions;

namespace Taarafo.Core.Models.GroupStudents.Exceptions
{
    public class NullGroupStudentException : Xeption
    {
        public NullGroupStudentException()
            : base(message: "Group student is null.")
        { }
    }
}
