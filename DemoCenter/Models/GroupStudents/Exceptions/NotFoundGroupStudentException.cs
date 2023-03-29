using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class NotFoundGroupStudentException : Xeption
    {
        public NotFoundGroupStudentException(Guid groupId, Guid studentId)
            : base(message: $"Couldn't find groupStudent with id: {groupId}, {studentId}.")
        { }
    }
}
