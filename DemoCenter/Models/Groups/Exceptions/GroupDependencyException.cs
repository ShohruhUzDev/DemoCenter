using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class GroupDependencyException : Xeption
    {
        public GroupDependencyException(Exception innerException)
            : base(message: "Group dependency error occured, contact support.", innerException)
        { }
    }
}
