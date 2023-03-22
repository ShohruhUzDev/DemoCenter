using System;
using Xeptions;

namespace DemoCenter.Models.Groups.Exceptions
{
    public class AlreadyExistGroupException : Xeption
    {
        public AlreadyExistGroupException(Exception innerException)
            : base(message: "Group already exist.", innerException)
        { }
    }
}
