using System;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTimeOffset();
            Group randomGroup = CreateRandomGroup(someDateTime);
            Group someGroup = randomGroup;
            Guid GroupId = someGroup.Id;
            SqlException sqlException = CreateSqlException();

            var failedGroupStorageException =
                new FailedGroupStorageException(sqlException);

            var expectedGroupDependencyException =
                new GroupDependencyException(failedGroupStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Throws(sqlException);

            // when
            ValueTask<Group> modifyGroupTask =
                this.groupService.ModifyGroupAsync(someGroup);

            GroupDependencyException actualGroupDependencyException =
                await Assert.ThrowsAsync<GroupDependencyException>(
                     modifyGroupTask.AsTask);

            // then
            actualGroupDependencyException.Should().BeEquivalentTo(
                expectedGroupDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptonAs(
                    expectedGroupDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(GroupId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupAsync(someGroup), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Group randomGroup = CreateRandomGroup(randomDateTime);
            Group someGroup = randomGroup;
            Guid GroupId = someGroup.Id;
            someGroup.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedGroupException =
                new FailedGroupStorageException(databaseUpdateException);

            var expectedGroupDependencyException =
                new GroupDependencyException(failedGroupException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(GroupId))
                    .ThrowsAsync(databaseUpdateException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Group> modifyGroupTask =
                this.groupService.ModifyGroupAsync(someGroup);

            GroupDependencyException actualGroupDependencyException =
                 await Assert.ThrowsAsync<GroupDependencyException>(
                     modifyGroupTask.AsTask);

            // then
            actualGroupDependencyException.Should().BeEquivalentTo(
                expectedGroupDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(GroupId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(
                    expectedGroupDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Group randomGroup = CreateRandomGroup(randomDateTime);
            Group someGroup = randomGroup;
            someGroup.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            Guid GroupId = someGroup.Id;
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedGroupException =
                new LockedGroupException(databaseUpdateConcurrencyException);

            var expectedGroupDependencyValidationException =
                new GroupDependencyValidationException(lockedGroupException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(GroupId))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Group> modifyGroupTask =
                this.groupService.ModifyGroupAsync(someGroup);

            GroupDependencyValidationException actualGroupDependencyValidationException =
                await Assert.ThrowsAsync<GroupDependencyValidationException>(
                    modifyGroupTask.AsTask);

            // then
            actualGroupDependencyValidationException.Should().BeEquivalentTo(
                expectedGroupDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(GroupId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(
                    expectedGroupDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            int minuteInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Group randomGroup = CreateRandomGroup(randomDateTime);
            Group someGroup = randomGroup;
            someGroup.CreatedDate = randomDateTime.AddMinutes(minuteInPast);
            var serviceException = new Exception();

            var failedGroupException =
                new FailedGroupServiceException(serviceException);

            var expectedGroupServiceException =
                new GroupServiceException(failedGroupException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(someGroup.Id))
                    .ThrowsAsync(serviceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Group> modifyGroupTask =
                this.groupService.ModifyGroupAsync(someGroup);

            GroupServiceException actualGroupServiceException =
                await Assert.ThrowsAsync<GroupServiceException>(
                    modifyGroupTask.AsTask);

            // then
            actualGroupServiceException.Should().BeEquivalentTo(
                expectedGroupServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(someGroup.Id), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(
                    expectedGroupServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
