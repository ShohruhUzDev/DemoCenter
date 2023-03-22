using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class GroupValidationException : Xeption
    {
        public GroupValidationException(Xeption innerException) :
            base(message: "Group validation error occured, fix the error and try again.", innerException)
        { }
    }
}
