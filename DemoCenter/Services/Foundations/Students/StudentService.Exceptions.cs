using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace DemoCenter.Services.Foundations.Students
{
    public partial class StudentService
    {
        private delegate ValueTask<Student> ReturningStudentFunction();
        private delegate IQueryable<Student> ReturningStudentFunctions();

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
            catch (InvalidStudentException invalidStudentException)
            {
                throw CreateAndLogValidationException(invalidStudentException);
            }
            catch (NotFoundStudentException notFoundStudentException)
            {
                throw CreateAndLogValidationException(notFoundStudentException);
            }
            catch (SqlException sqlException)
            {
                var failedStudentStorageException = new FailedStudentStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedStudentStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsStudentException = new AlreadyExistsStudentException(duplicateKeyException);

                throw CreateAndDependencyValidationException(alreadyExistsStudentException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedStudentException = new LockedStudentException(dbUpdateConcurrencyException);

                throw CreateAndDependencyValidationException(lockedStudentException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStudentStorageException = new FailedStudentStorageException(dbUpdateException);

                throw CreateAndLogDependencyException(failedStudentStorageException);
            }
            catch (Exception exception)
            {
                var failedStudentServiceException = new FailedStudentServiceException(exception);

                throw CreateAndLogServiceException(failedStudentServiceException);
            }
        }

        private IQueryable<Student> TryCatch(ReturningStudentFunctions returningStudentFunctions)
        {
            try
            {
                return returningStudentFunctions();
            }
            catch (SqlException sqlException)
            {
                var failedStudentStorageException = new FailedStudentStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedStudentStorageException);
            }
            catch (Exception exception)
            {
                var failedStudentServiceException = new FailedStudentServiceException(exception);

                throw CreateAndLogServiceException(failedStudentServiceException);
            }
        }
        private StudentDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var studentDependencyException = new StudentDependencyException(exception);
            this.loggingBroker.LogError(studentDependencyException);

            return studentDependencyException;
        }
        private StudentServiceException CreateAndLogServiceException(Exception exception)
        {
            var studentServiceException = new StudentServiceException(exception);
            this.loggingBroker.LogError(studentServiceException);

            return studentServiceException;
        }

        private StudentDependencyValidationException CreateAndDependencyValidationException(Xeption exception)
        {
            var studentDependencyValidationException = new StudentDependencyValidationException(exception);
            this.loggingBroker.LogError(studentDependencyValidationException);

            return studentDependencyValidationException;
        }
        private StudentDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var studentDependencyException = new StudentDependencyException(exception);
            this.loggingBroker.LogCritical(studentDependencyException);

            return studentDependencyException;
        }
        private StudentValidationException CreateAndLogValidationException(Xeption exception)
        {
            var studentValidationException = new StudentValidationException(exception);
            this.loggingBroker.LogError(studentValidationException);

            throw studentValidationException;
        }
    }
}
