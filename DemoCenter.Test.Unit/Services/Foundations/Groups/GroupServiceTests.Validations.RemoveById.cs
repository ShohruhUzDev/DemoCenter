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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidGroupId = Guid.Empty;

            var invalidGroupException = new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.Id),
                values: "Id is required");

            var expectedGroupValidationException = new GroupValidationException(invalidGroupException);

            //when
            ValueTask<Group> onRemoveGroupTask = this.groupService.RemoveGroupByIdAsync(invalidGroupId);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(onRemoveGroupTask.AsTask);

            //then
            actualGroupValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGroupAsync(It.IsAny<Group>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFounExceptionOnRetrieveIfGroupIsNotFoundAndLogItAsync()
        {
            //given
            Guid randomGroupId = Guid.NewGuid();
            Guid inputGroupId = randomGroupId;
            Group noGroup = null;
            var notFoundGroupException = new NotFoundGroupException(inputGroupId);

            var expectedGroupValidationException =
                new GroupValidationException(notFoundGroupException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noGroup);

            //when
            ValueTask<Group> onRemoveGroupTask = this.groupService.RemoveGroupByIdAsync(inputGroupId);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(onRemoveGroupTask.AsTask);

            //then
            actualGroupValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
