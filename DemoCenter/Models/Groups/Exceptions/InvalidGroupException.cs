using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class InvalidGroupException :Xeption
    {
        public InvalidGroupException()
            :base(message : "Group is invalid.")
        {           
        }
    }
}