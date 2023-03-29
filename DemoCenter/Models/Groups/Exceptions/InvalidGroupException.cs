using Xeptions;

namespace DemoCenter.Models.GroupStudents.Exceptions
{
    public class InvalidGroupException : Xeption
    {
        public InvalidGroupException()
            : base(message: "Group is invalid.")
        {
        }
    }
}