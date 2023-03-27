using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace DemoCenter.Services.Foundations.Subjects
{
    public partial class SubjectService
    {
        private delegate ValueTask<Subject> ReturningSubjectFunction();
        private delegate IQueryable<Subject> ReturningSubjectFunctions();

        private async ValueTask<Subject> TryCatch(ReturningSubjectFunction returningSubjectFunction)
        {
            try
            {
                return await returningSubjectFunction();
            }
            catch (NullSubjectException nulSubjectException)
            {

                throw CreateAndLogValidationExcetion(nulSubjectException);
            }
            catch (InvalidSubjectException invalidSubjectException)
            {

                throw CreateAndLogValidationExcetion(invalidSubjectException);
            }
            catch (NotFoundSubjectException notFoundSubjectException)
            {

                throw CreateAndLogValidationExcetion(notFoundSubjectException);
            }
            catch (SqlException sqlException)
            {
                var failedSubjectStorageException = new FailedSubjectStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedSubjectStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsSubjectException = new AlreadyExistsSubjectException(duplicateKeyException);

                throw CreateAndDependencyValidationException(alreadyExistsSubjectException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedSubjectException = new LockedSubjectException(dbUpdateConcurrencyException);

                throw CreateAndDependencyValidationException(lockedSubjectException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedSubjectStorageException = new FailedSubjectStorageException(dbUpdateException);

                throw CreateAndDependencyException(failedSubjectStorageException);
            }
            catch (Exception exception)
            {
                var failedSubjectServiceException = new FailedSubjectServiceException(exception);

                throw CreateAndLogServiceException(failedSubjectServiceException);
            }
        }

        private IQueryable<Subject> TryCatch(ReturningSubjectFunctions returningSubjectFunctions)
        {
            try
            {
                return returningSubjectFunctions();
            }
            catch (SqlException sqlException)
            {
                var failedSubjectStorageException = new FailedSubjectStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedSubjectStorageException);
            }
            catch (Exception exception)
            {
                var failedSubjectServiceException = new FailedSubjectServiceException(exception);

                throw CreateAndLogServiceException(failedSubjectServiceException);
            }
        }
        private SubjectDependencyException CreateAndDependencyException(Xeption exception)
        {
            var subjectDependencyException = new SubjectDependencyException(exception);
            this.loggingBroker.LogError(subjectDependencyException);

            return subjectDependencyException;
        }
        private SubjectServiceException CreateAndLogServiceException(Exception exception)
        {
            var subjectServceException = new SubjectServiceException(exception);
            this.loggingBroker.LogError(subjectServceException);

            return subjectServceException;
        }
        private SubjectDependencyValidationException CreateAndDependencyValidationException(Xeption exception)
        {
            var subjectDependencyValidationException = new SubjectDependencyValidationException(exception);
            this.loggingBroker.LogError(subjectDependencyValidationException);

            return subjectDependencyValidationException;
        }
        private SubjectDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var subjectDependencyException = new SubjectDependencyException(exception);
            this.loggingBroker.LogCritical(subjectDependencyException);

            return subjectDependencyException;
        }

        private SubjectValidationException CreateAndLogValidationExcetion(Xeption xeption)
        {
            var subjectValidationException =
                new SubjectValidationException(xeption);

            this.loggingBroker.LogError(subjectValidationException);

            return subjectValidationException;
        }
    }
}
