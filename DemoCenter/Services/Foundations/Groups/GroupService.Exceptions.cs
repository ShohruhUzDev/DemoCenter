using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Groups;
using DemoCenter.Models.Foundations.Groups.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace DemoCenter.Services.Foundations.Groups
{
    public partial class GroupService
    {
        private delegate ValueTask<Group> ReturningGroupFunction();
        private delegate IQueryable<Group> ReturningGroupsFunction();
        private async ValueTask<Group> TryCatch(ReturningGroupFunction returningGroupFunction)
        {
            try
            {
                return await returningGroupFunction();
            }
            catch (NullGroupException nullGroupException)
            {
                throw CreateAndLogValidationException(nullGroupException);
            }
            catch (InvalidGroupException invalidGroupException)
            {
                throw CreateAndLogValidationException(invalidGroupException);
            }
            catch (NotFoundGroupException notFoundGroupException)
            {
                throw CreateAndLogValidationException(notFoundGroupException);
            }
            catch (SqlException sqlException)
            {
                var failedGroupStorageException = new FailedGroupStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedGroupStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var failedGroupDependencyValidationException =
                     new AlreadyExistGroupException(duplicateKeyException);

                throw CreateAndDependencyValidationException(failedGroupDependencyValidationException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidGroupReferenceException =
                    new InvalidGroupReferenceException(foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidGroupReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedGroupException = new LockedGroupException(dbUpdateConcurrencyException);

                throw CreateAndDependencyValidationException(lockedGroupException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedGroupStorageException = new FailedGroupStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedGroupStorageException);
            }
            catch (Exception serviceException)
            {
                var failedGroupServiceException = new FailedGroupServiceException(serviceException);

                throw CreateAndLogServiceException(failedGroupServiceException);
            }
        }

        private IQueryable<Group> TryCatch(ReturningGroupsFunction returningGroupsFunction)
        {
            try
            {
                return returningGroupsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedGroupStorageException = new FailedGroupStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedGroupStorageException);
            }
            catch (Exception serviceException)
            {
                var failedGroupServiceException = new FailedGroupServiceException(serviceException);

                throw CreateAndLogServiceException(failedGroupServiceException);
            }
        }
        private GroupDependencyValidationException CreateAndLogDependencyValidationException(
        Xeption exception)
        {
            var groupDependencyValidationException =
                new GroupDependencyValidationException(exception);

            this.loggingBroker.LogError(groupDependencyValidationException);

            return groupDependencyValidationException;
        }
        private GroupDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var groupDependencyException = new GroupDependencyException(exception);
            this.loggingBroker.LogError(groupDependencyException);

            return groupDependencyException;
        }

        private GroupServiceException CreateAndLogServiceException(Exception exception)
        {
            var groupServiceException = new GroupServiceException(exception);
            this.loggingBroker.LogError(groupServiceException);

            return groupServiceException;
        }
        private GroupDependencyValidationException CreateAndDependencyValidationException(Xeption exception)
        {
            var groupDependencyValidationException = new GroupDependencyValidationException(exception);
            this.loggingBroker.LogError(groupDependencyValidationException);

            return groupDependencyValidationException;
        }
        private GroupValidationException CreateAndLogValidationException(Xeption exception)
        {
            var groupValidationException = new GroupValidationException(exception);
            this.loggingBroker.LogError(groupValidationException);

            throw groupValidationException;
        }
        private GroupDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var groupDependencyException = new GroupDependencyException(exception);
            this.loggingBroker.LogCritical(groupDependencyException);

            return groupDependencyException;
        }
    }
}
