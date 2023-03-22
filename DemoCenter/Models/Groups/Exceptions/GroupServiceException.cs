using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class GroupServiceException : Xeption
    {
        public GroupServiceException(Exception innerException)
            : base(message: "Group service error occured, contact support.", innerException)
        { }
    }
}
