using System;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDepedencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Group someGroup = CreateRandomGroup();
            SqlException sqlException = CreateSqlException();
            var failedGroupStorageException = new FailedGroupStorageException(sqlException);

            var expectedDependencyException =
                new GroupDependencyException(failedGroupStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Throws(sqlException);

          
            //when
            ValueTask<Group> addGroupTask=this.groupService.AddGroupAsync(someGroup);

            GroupDependencyException actualGroupDependencyException =
                await Assert.ThrowsAsync<GroupDependencyException>(addGroupTask.AsTask);

            //then
            actualGroupDependencyException.Should().BeEquivalentTo(expectedDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptonAs(
                    expectedDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldTrowDependencyValidationExceptionOnAddIfDuplicateErrorOccursAndLogItAsync()
        {
            // given
            Group someGroup = CreateRandomGroup();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsGroupException =
                new AlreadyExistGroupException(duplicateKeyException);

            var expectedGroupDependencyValidationException =
                new GroupDependencyValidationException(alreadyExistsGroupException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrenDateTime())
                .Throws(duplicateKeyException);

            // when
            ValueTask<Group> addGroupTask = this.groupService.AddGroupAsync(someGroup);

            GroupDependencyValidationException actualGroupDependencyValidationException =
                await Assert.ThrowsAsync<GroupDependencyValidationException>(addGroupTask.AsTask);

            // then
            actualGroupDependencyValidationException.Should().BeEquivalentTo(
                expectedGroupDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptonAs(
                expectedGroupDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker => broker.InsertGroupAsync(
               It.IsAny<Group>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDbConcurrencyErrorOccursAndLogItAsync()
        {
            //given
            Group someGroup = CreateRandomGroup();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();
            var lockedGroupException = new LockedGroupException(dbUpdateConcurrencyException);

            var expectedGroupDependencyValidationException =
                new GroupDependencyValidationException(lockedGroupException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrenDateTime())
                .Throws(dbUpdateConcurrencyException);

            //when
            ValueTask<Group> addGroupTask = this.groupService.AddGroupAsync(someGroup);

            GroupDependencyValidationException actualGroupDependencyValidationException =
                await Assert.ThrowsAsync<GroupDependencyValidationException>(addGroupTask.AsTask);

            //then
            actualGroupDependencyValidationException.Should()
                .BeEquivalentTo(expectedGroupDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptonAs(expectedGroupDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupAsync(It.IsAny<Group>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Group someGroup = CreateRandomGroup();
            var serviceException = new Exception();

            var failedGroupServiceException =
                new FailedGroupServiceException(serviceException);

            var expectedGroupServiceException =
                new GroupServiceException(failedGroupServiceException);

            this.dateTimeBrokerMock.Setup(broker => 
                broker.GetCurrenDateTime())
                    .Throws(serviceException);

            //when
            ValueTask<Group> addGroupTask =
                this.groupService.AddGroupAsync(someGroup);

            GroupServiceException actualGroupServiceException =
                await Assert.ThrowsAsync<GroupServiceException>(addGroupTask.AsTask);

            //then
            actualGroupServiceException.Should().BeEquivalentTo(expectedGroupServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(
                    expectedGroupServiceException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupAsync(It.IsAny<Group>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
