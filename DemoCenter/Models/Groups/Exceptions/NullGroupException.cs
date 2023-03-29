using Xeptions;

namespace DemoCenter.Models.GroupStudents.Exceptions
{
    public class NullGroupException : Xeption
    {
        public NullGroupException() :
            base(message: "Group is null.")
        {

        }
    }
}
