using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class NullGroupException : Xeption
    {
        public NullGroupException() :
            base(message:"Group is null.")
        {
            
        }
    }
}
