using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class NotFoundGroupException : Xeption
    {
        public NotFoundGroupException(Guid groupId) :
            base(message: $"Couldn't find group with id: {groupId}")
        {
        }
    }
}
