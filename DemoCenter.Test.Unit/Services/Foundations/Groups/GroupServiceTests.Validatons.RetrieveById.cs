using System;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidGroupId = Guid.Empty;
            var invalidGroupException = new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.Id),
                values: "Id is required");

            var expectedGroupValidationException =
                new GroupValidationException(invalidGroupException);

            //when
            ValueTask<Group> onRetrieveGroupTask = this.groupService.RetrieveGroupByIdAsync(invalidGroupId);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(onRetrieveGroupTask.AsTask);

            //then
            actualGroupValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfGroupNotFoundAndLogItAsync()
        {
            //given
            Guid someGroupId = Guid.NewGuid();
            Group noGroup = null;
            var notFoundGroupValidationException = new NotFoundGroupException(someGroupId);

            var expectedGroupValidationException =
                new GroupValidationException(notFoundGroupValidationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noGroup);

            //when
            ValueTask<Group> onRetrieveGroupByIdTask =
                this.groupService.RetrieveGroupByIdAsync(someGroupId);
            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(onRetrieveGroupByIdTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
