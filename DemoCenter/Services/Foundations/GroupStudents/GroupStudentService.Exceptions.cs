using System.Threading.Tasks;
using DemoCenter.Models.GroupStudents.Exceptions;
using DemoCenter.Models.GroupStudents;
using Xeptions;

namespace DemoCenter.Services.Foundations.GroupStudents
{
    public partial class GroupStudentService
    {
        private delegate ValueTask<GroupStudent> ReturningGroupStudentFunction();

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

        }
        private GroupStudentValidationException CreateAndLogValidationException(Xeption exception)
        {
            var groupStudentValidationException = new GroupStudentValidationException(exception);
            this.loggingBroker.LogError(groupStudentValidationException);

            return groupStudentValidationException;
        }
    }
}
