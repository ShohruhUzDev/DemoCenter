//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Xeptions;

namespace Tarteeb.Api.Services.Foundations.Securities
{
    public partial class SecurityService
    {
        private delegate string ReturningTokenFunction();

        private string TryCatch(ReturningTokenFunction returningTokenFunction)
        {
            try
            {
                return returningTokenFunction();
            }
            catch (NullUserException nullUserException)
            {
                throw CreateAndLogValidationException(nullUserException);
            }
            catch (InvalidUserException invalidUserException)
            {
                throw CreateAndLogValidationException(invalidUserException);
            }
            catch (Exception serviceException)
            {
                var failedUserServiceException =
                    new FailedUserServiceException(serviceException);

                throw CreateAndLogServiceException(failedUserServiceException);
            }

        }

        private UserValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userValidationException =
                new UserValidationException(exception);

            this.loggingBroker.LogError(userValidationException);

            return userValidationException;
        }

        private UserServiceException CreateAndLogServiceException(Xeption exception)
        {
            var userServiceException =
                new UserServiceException(exception);

            this.loggingBroker.LogError(userServiceException);

            return userServiceException;
        }
    }
}
