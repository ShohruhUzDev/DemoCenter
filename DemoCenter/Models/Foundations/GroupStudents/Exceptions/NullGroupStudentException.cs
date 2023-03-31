using Xeptions;

namespace DemoCenter.Models.Foundations.GroupStudents.Exceptions
{
    public class NullGroupStudentException : Xeption
    {
        public NullGroupStudentException()
            : base(message: "Group student is null.")
        { }
    }
}
