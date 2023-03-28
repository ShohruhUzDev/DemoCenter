using System;
using Xeptions;

namespace Taarafo.Core.Models.GroupPosts.Exceptions
{
    public class FailedGroupStudentStorageException : Xeption
    {
        public FailedGroupStudentStorageException(Exception innerException)
            : base(message: "Failed group student storage error occured, contact support.", innerException)
        { }
    }
}
