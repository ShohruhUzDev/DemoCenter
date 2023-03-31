using System;
using Xeptions;

namespace DemoCenter.Models.Foundations.Subjects.Exceptions
{
    public class AlreadyExistsSubjectException : Xeption
    {
        public AlreadyExistsSubjectException(Exception innerException)
            : base(message: "Subject already exist.", innerException)
        { }
    }
}
