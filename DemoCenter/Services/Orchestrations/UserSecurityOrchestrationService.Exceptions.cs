
using System;
using DemoCenter.Models.Foundations.Users.Exceptions;
using DemoCenter.Models.Orchestrations.Exceptions;
using DemoCenter.Models.Orchestrations.UserTokens;
using Xeptions;

namespace DemoCenter.Services.Orchestrations
{
    public partial class UserSecurityOrchestrationService
    {
        private delegate UserToken ReturningUserTokenFunction();

        private UserToken TryCatch(ReturningUserTokenFunction returningUserTokenFunction)
        {
            try
            {
                return returningUserTokenFunction();
            }
            catch (InvalidUserCredentialOrchestrationException invalidUserCreadentialOrchestrationException)
            {
                throw CreateAndLogValidationException(invalidUserCreadentialOrchestrationException);
            }
            catch (NotFoundUserException notFoundUserException)
            {
                throw CreateAndLogValidationException(notFoundUserException);
            }
            catch (UserDependencyException userDependencyException)
            {
                throw CreateAndLogDependencyException(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                throw CreateAndLogDependencyException(userServiceException);
            }
            catch (Exception exception)
            {
                var failedUserTokenOrchestrationException =
                    new FailedUserTokenOrchestrationException(exception);

                throw CreateAndLogServiceException(failedUserTokenOrchestrationException);
            }
        }

        private UserTokenOrchestrationValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userTokenOrchestrationValidationException = new UserTokenOrchestrationValidationException(exception);
            this.loggingBroker.LogError(userTokenOrchestrationValidationException);

            return userTokenOrchestrationValidationException;
        }

        private UserTokenOrchestrationDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var userTokenOrchestrationDependencyException = new UserTokenOrchestrationDependencyException(exception);
            this.loggingBroker.LogError(userTokenOrchestrationDependencyException);

            return userTokenOrchestrationDependencyException;
        }

        private UserTokenOrchestrationServiceException CreateAndLogServiceException(Xeption exception)
        {
            var userTokenOrchestrationServiceException =
                new UserTokenOrchestrationServiceException(exception);

            this.loggingBroker.LogError(userTokenOrchestrationServiceException);

            return userTokenOrchestrationServiceException;
        }
    }
}
