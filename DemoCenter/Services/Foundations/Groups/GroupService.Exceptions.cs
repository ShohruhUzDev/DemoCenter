using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
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
        }

        private GroupValidationException CreateAndLogValidationException(Xeption exception)
        {
            var groupValidationException = new GroupValidationException(exception);
            this.loggingBroker.LogError(groupValidationException);

            throw groupValidationException;
        }
    }
}
