using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoCenter.Models.Users;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            User noUser = null;
            var nullUserException = new NullUserException();

            var expectedUserValidationexception =
                new UserValidationException(nullUserException);

            // when
            ValueTask<User> addUserTask =
                this.userService.AddUserAsync(noUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(addUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(
               expectedUserValidationexception);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationexception))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfUserIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            var invalidUser = new User
            {
                FirstName = invalidString
            };

            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            invalidUserException.AddData(
                key: nameof(User.FirstName),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.LastName),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.Email),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.BirthDate),
                values: "Value is required");

            invalidUserException.AddData(
                key: nameof(User.CreatedDate),
                values: "Value is required");

            invalidUserException.AddData(
                key: nameof(User.UpdatedDate),
                values: "Value is required");

            invalidUserException.AddData(
                key: nameof(User.Password),
                values: "Text is required");

            var expectedUserValidationException = new UserValidationException(
                invalidUserException);

            // when
            ValueTask<User> addUserTask = this.userService.AddUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(addUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }



    }
}
