using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace DemoCenter.Services.Foundations.Groups
{
    public partial class GroupService
    {
        private delegate ValueTask<Group> ReturningGroupFunction();

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
                var failedTicketDependencyValidationException =
                     new AlreadyExistGroupException(duplicateKeyException);

                throw CreateAndDependencyValidationException(failedTicketDependencyValidationException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedGroupException = new LockedGroupException(dbUpdateConcurrencyException);

                throw CreateAndDependencyValidationException(lockedGroupException);
            }
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
            var groupDependencyException=new GroupDependencyException(exception);   
            this.loggingBroker.LogCritical(groupDependencyException);

            return groupDependencyException;    
        }
    }
}
