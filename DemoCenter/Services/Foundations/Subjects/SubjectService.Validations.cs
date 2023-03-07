using System.Security.Cryptography.Xml;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;

namespace DemoCenter.Services.Foundations.Subjects
{
    public partial class SubjectService
    {
        private static void ValidateSubjectNotNull(Subject subject)
        {
            if(subject is null)
            {
                throw new NullSubjectException();
            }
        }
    }
}
