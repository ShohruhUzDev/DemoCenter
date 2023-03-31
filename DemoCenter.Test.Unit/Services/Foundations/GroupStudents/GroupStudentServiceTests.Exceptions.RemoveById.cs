using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.GroupStudents;
using DemoCenter.Models.Foundations.GroupStudents.Exceptions;
using DemoCenter.Models.GroupStudents.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid groupId = Guid.NewGuid();
            Guid studentId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedGroupStudentException =
                new LockedGroupStudentException(databaseUpdateConcurrencyException);

            var expectedGroupStudentDependencyValidationException =
                new GroupStudentDependencyValidationException(lockedGroupStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<GroupStudent> removeGroupStudentByIdTask =
                this.groupStudentService.RemoveGroupStudentByIdAsync(groupId, studentId);

            GroupStudentDependencyValidationException actualGroupStudentDependencyValidationException =
                await Assert.ThrowsAsync<GroupStudentDependencyValidationException>(
                    removeGroupStudentByIdTask.AsTask);

            // then
            actualGroupStudentDependencyValidationException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnDeleteWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid groupId = Guid.NewGuid();
            Guid studentId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedGroupStudentStorageException =
                new FailedGroupStudentStorageException(sqlException);

            var expectedGroupStudentDependencyException =
                new GroupStudentDependencyException(failedGroupStudentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<GroupStudent> deleteGroupStudentTask =
                this.groupStudentService.RemoveGroupStudentByIdAsync(groupId, studentId);

            GroupStudentDependencyException actualGroupStudentDependencyException =
                await Assert.ThrowsAsync<GroupStudentDependencyException>(
                    deleteGroupStudentTask.AsTask);

            // then
            actualGroupStudentDependencyException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid groupId = Guid.NewGuid();
            Guid studentId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedGroupStudentServiceException =
                new FailedGroupStudentServiceException(serviceException);

            var expectedGroupStudentServiceException =
                new GroupStudentServiceException(failedGroupStudentServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<GroupStudent> removeGroupStudentByIdTask =
                this.groupStudentService.RemoveGroupStudentByIdAsync(groupId, studentId);

            GroupStudentServiceException actualGroupStudentServiceException =
                await Assert.ThrowsAsync<GroupStudentServiceException>(
                    removeGroupStudentByIdTask.AsTask);

            // then
            actualGroupStudentServiceException.Should().BeEquivalentTo(
                expectedGroupStudentServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
