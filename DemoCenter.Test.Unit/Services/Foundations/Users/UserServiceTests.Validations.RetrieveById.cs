using System;
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
        public async Task ShouldThrowVlidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invlidUserId = Guid.Empty;
            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            // when
            ValueTask<User> retrieveUserByIdTask =
                this.userService.RetrieveUserByIdAsync(invlidUserId);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(retrieveUserByIdTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfUserIsNotFoundAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            User noUser = null;
            var notFoundUserValidationException = new NotFoundUserException(someUserId);

            var expectedValidationException =
                new UserValidationException(notFoundUserValidationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noUser);

            // when
            ValueTask<User> retrieveByIdUserTask =
                this.userService.RetrieveUserByIdAsync(someUserId);

            UserValidationException actualValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    retrieveByIdUserTask.AsTask);

            // then
            actualValidationException.Should().BeEquivalentTo(expectedValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
