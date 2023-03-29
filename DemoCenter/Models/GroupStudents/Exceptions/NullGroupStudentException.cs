using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class NullGroupStudentException : Xeption
    {
        public NullGroupStudentException()
            : base(message: "Group student is null.")
        { }
    }
}
