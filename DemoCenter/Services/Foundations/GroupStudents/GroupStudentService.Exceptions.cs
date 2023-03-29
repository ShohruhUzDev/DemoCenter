using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Models.Groups.Exceptions;
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

        }
        private GroupStudentValidationException CreateAndLogValidationException(Xeption exception)
        {
            var groupStudentValidationException = new GroupStudentValidationException(exception);
            this.loggingBroker.LogError(groupStudentValidationException);

            return groupStudentValidationException;
        }
    }
}
