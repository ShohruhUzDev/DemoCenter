using System;
using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class NotFoundSubjectException : Xeption
    {
        public NotFoundSubjectException(Guid subjectId) :
            base(message: $"Couldn't found subject with id: {subjectId}")
        {
        }
    }
}
