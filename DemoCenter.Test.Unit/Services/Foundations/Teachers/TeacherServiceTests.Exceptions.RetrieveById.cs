using System;
using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedTeacherStorageException =
                new FailedTeacherStorageException(sqlException);

            var expectedTeacherDependencyException =
                new TeacherDependencyException(failedTeacherStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Teacher> retrieveTeacherByIdTask =
                this.teacherService.RetrieveTeacherByIdAsync(someId);

            TeacherDependencyException actaulTeacherDependencyException =
                await Assert.ThrowsAsync<TeacherDependencyException>(
                    retrieveTeacherByIdTask.AsTask);

            // then
            actaulTeacherDependencyException.Should().BeEquivalentTo(
                expectedTeacherDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTeacherDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedTeacherServiceException =
                new FailedTeacherServiceException(serviceException);

            var expectedTeacherServiceExcpetion =
                new TeacherServiceException(failedTeacherServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<Teacher> retrieveTeacherById =
            this.teacherService.RetrieveTeacherByIdAsync(someId);

            TeacherServiceException actualTeacherServiceException =
                await Assert.ThrowsAsync<TeacherServiceException>(retrieveTeacherById.AsTask);

            // then
            actualTeacherServiceException.Should().BeEquivalentTo(expectedTeacherServiceExcpetion);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedTeacherServiceExcpetion))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
