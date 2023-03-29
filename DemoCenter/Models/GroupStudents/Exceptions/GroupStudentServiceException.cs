using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class GroupStudentServiceException : Xeption
    {
        public GroupStudentServiceException(Exception innerException)
            : base(message: "Group student service error occurred, please contact support.", innerException)
        { }
    }
}
