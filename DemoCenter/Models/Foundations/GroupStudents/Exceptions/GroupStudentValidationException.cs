﻿
using Xeptions;

namespace DemoCenter.Models.GroupStudents.Exceptions
{
    public class GroupStudentValidationException : Xeption
    {
        public GroupStudentValidationException(Xeption innerException)
            : base(message: "Group student validation error occurred, please try again.",
                  innerException)
        { }
    }
}
