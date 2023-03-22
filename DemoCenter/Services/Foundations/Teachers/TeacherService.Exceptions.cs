using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using Microsoft.Data.SqlClient;
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
            catch (NotFoundTeacherException notFoundTeacherException)
            {

                throw CreateTeAndLogValidationException(notFoundTeacherException);
            }
            catch (SqlException sqlException)
            {
                var failedTeacherStorageException = new FailedTeacherStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedTeacherStorageException);
            }

        }
        private TeacherDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var teacherDependencyException = new TeacherDependencyException(exception);
            this.loggingBroker.LogCritical(teacherDependencyException);

            return teacherDependencyException;
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
