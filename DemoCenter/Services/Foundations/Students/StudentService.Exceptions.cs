using System;
using System.Threading.Tasks;
using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;
using Xeptions;

namespace DemoCenter.Services.Foundations.Students
{
    public partial class StudentService
    {
        private delegate ValueTask<Student> ReturningStudentFunction();

        private async ValueTask<Student> TryCatch(ReturningStudentFunction returningStudentFunction)
        {
            try
            {
                return await returningStudentFunction();
            }
            catch (NullStudentException nullStudentException)
            {

                throw CreateAndLogValidationException(nullStudentException);
            }
        }

        private StudentValidationException CreateAndLogValidationException(Xeption exception)
        {
           var studentValidationException=new StudentValidationException(exception);
            this.loggingBroker.LogError(studentValidationException);

            throw studentValidationException;
        }
    }
}
