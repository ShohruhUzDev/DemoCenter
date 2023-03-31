using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.GroupStudents;
using DemoCenter.Models.Foundations.GroupStudents.Exceptions;
using DemoCenter.Models.GroupStudents.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someGroupId = Guid.NewGuid();
            Guid someStudentId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedGroupStudentStorageException =
                new FailedGroupStudentStorageException(sqlException);

            var expectedGroupStudentDependencyException =
                new GroupStudentDependencyException(failedGroupStudentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(someGroupId, someStudentId))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<GroupStudent> retrieveGroupStudentByIdTask =
                this.groupStudentService.RetrieveGroupStudentByIdAsync(someGroupId, someStudentId);

            GroupStudentDependencyException actualGroupStudentDependencyException =
                await Assert.ThrowsAsync<GroupStudentDependencyException>(
                    retrieveGroupStudentByIdTask.AsTask);

            //then
            actualGroupStudentDependencyException.Should().BeEquivalentTo(
                   expectedGroupStudentDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), (It.IsAny<Guid>())),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someGroupId = Guid.NewGuid();
            Guid someStudentId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedGroupStudentServiceException =
                new FailedGroupStudentServiceException(serviceException);

            var expectedGroupStudentServiceException =
                new GroupStudentServiceException(failedGroupStudentServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(someGroupId, someStudentId))
                    .ThrowsAsync(serviceException);

            //when
            ValueTask<GroupStudent> retrieveGroupStudentByIdTask =
                this.groupStudentService.RetrieveGroupStudentByIdAsync(someGroupId, someStudentId);

            GroupStudentServiceException actualGroupStudentServiceException =
                 await Assert.ThrowsAsync<GroupStudentServiceException>(retrieveGroupStudentByIdTask.AsTask);

            //then
            actualGroupStudentServiceException.Should().BeEquivalentTo(expectedGroupStudentServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), (It.IsAny<Guid>())),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
