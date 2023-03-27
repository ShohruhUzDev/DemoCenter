using System;
using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class AlreadyExistsSubjectException : Xeption
    {
        public AlreadyExistsSubjectException(Exception innerException)
            : base(message: "Subject already exist.", innerException)
        { }
    }
}
