using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using Xeptions;

namespace DemoCenter.Services.Foundations.Teachers
{
    public partial class TeacherService
    {
        private delegate ValueTask<Teacher> ReturningTeacherFunction();

        private async ValueTask<Teacher> TryCatch(ReturningTeacherFunction returningTeacherFunction)
        {
            try
            {
                return await returningTeacherFunction();
            }
            catch (NullTeacherException nullTeacherException)
            {

                throw CreateTeAndLogValidationException(nullTeacherException);
            }
            catch (InvalidTeacherException invalidTeacherException)
            {

                throw CreateTeAndLogValidationException(invalidTeacherException);
            }

        }

        private TeacherValidationException CreateTeAndLogValidationException(Xeption xeption)
        {
            var teacherValidationException =
                new TeacherValidationException(xeption);

            this.loggingBroker.LogError(teacherValidationException);

            return teacherValidationException;
        }
    }
}
