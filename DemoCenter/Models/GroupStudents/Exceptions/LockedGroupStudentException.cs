using System;
using Xeptions;

namespace Taarafo.Core.Models.GroupPosts.Exceptions
{
    public class LockedGroupStudentException : Xeption
    {
        public LockedGroupStudentException(Exception innerException)
            : base(message: "GrouStudent is locked, please try again.", innerException)
        { }
    }
}