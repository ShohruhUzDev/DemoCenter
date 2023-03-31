using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.GroupStudents;
using DemoCenter.Models.Foundations.GroupStudents.Exceptions;
using DemoCenter.Models.Groups.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace DemoCenter.Services.Foundations.GroupStudents
{
    public partial class GroupStudentService
    {
        private delegate ValueTask<GroupStudent> ReturningGroupStudentFunction();
        private delegate IQueryable<GroupStudent> ReturningGroupStudentsFunction();

        private async ValueTask<GroupStudent> TryCatch(ReturningGroupStudentFunction returningGroupStudentFunction)
        {
            try
            {
                return await returningGroupStudentFunction();
            }
            catch (NullGroupStudentException nullGroupStudentException)
            {
                throw CreateAndLogValidationException(nullGroupStudentException);
            }
            catch (InvalidGroupStudentException invalidGroupStudentException)
            {
                throw CreateAndLogValidationException(invalidGroupStudentException);
            }
            catch (NotFoundGroupStudentException notFoundGroupStudentException)
            {
                throw CreateAndLogValidationException(notFoundGroupStudentException);
            }
            catch (SqlException sqlException)
            {
                var failedGroupStudentStorageException =
                    new FailedGroupStudentStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedGroupStudentStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var failedGroupStudentDependencyValidationException =
                     new AlreadyExistsGroupStudentException(duplicateKeyException);

                throw CreateAndDependencyValidationException(failedGroupStudentDependencyValidationException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedGroupStudentException = new LockedGroupStudentException(dbUpdateConcurrencyException);

                throw CreateAndDependencyValidationException(lockedGroupStudentException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedGroupStudentStorageException = new FailedGroupStudentStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedGroupStudentStorageException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidGroupStudentReferenceException =
                    new InvalidGroupStudentReferenceException(foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidGroupStudentReferenceException);
            }
            catch (Exception serviceException)
            {
                var failedGroupStudentServiceException = new FailedGroupStudentServiceException(serviceException);

                throw CreateAndLogServiceException(failedGroupStudentServiceException);
            }

        }

        private IQueryable<GroupStudent> TryCatch(ReturningGroupStudentsFunction returningGroupStudentsFunction)
        {
            try
            {
                return returningGroupStudentsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedGroupStudentStorageException = new FailedGroupStudentStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedGroupStudentStorageException);
            }
            catch (Exception serviceException)
            {
                var failedGroupStudentServiceException = new FailedGroupStudentServiceException(serviceException);

                throw CreateAndLogServiceException(failedGroupStudentServiceException);
            }
        }
        private GroupStudentServiceException CreateAndLogServiceException(Exception exception)
        {
            var groupStudentServiceException = new GroupStudentServiceException(exception);
            this.loggingBroker.LogError(groupStudentServiceException);

            return groupStudentServiceException;
        }

        private GroupStudentDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var groupStudentDependencyValidationException =
                new GroupStudentDependencyValidationException(exception);

            this.loggingBroker.LogError(groupStudentDependencyValidationException);

            return groupStudentDependencyValidationException;
        }

        private GroupStudentDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var groupStudentDependencyException = new GroupStudentDependencyException(exception);
            this.loggingBroker.LogError(groupStudentDependencyException);

            return groupStudentDependencyException;
        }

        private GroupStudentDependencyValidationException CreateAndDependencyValidationException(Xeption exception)
        {
            var groupStudentDependencyValidationException = new GroupStudentDependencyValidationException(exception);
            this.loggingBroker.LogError(groupStudentDependencyValidationException);

            return groupStudentDependencyValidationException;
        }

        private GroupStudentDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var groupStudentDependencyException = new GroupStudentDependencyException(exception);
            this.loggingBroker.LogCritical(groupStudentDependencyException);

            return groupStudentDependencyException;
        }

        private GroupStudentValidationException CreateAndLogValidationException(Xeption exception)
        {
            var groupStudentValidationException = new GroupStudentValidationException(exception);
            this.loggingBroker.LogError(groupStudentValidationException);

            return groupStudentValidationException;
        }
    }
}
