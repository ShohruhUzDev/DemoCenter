using Microsoft.Data.SqlClient;
using Moq;
using System.Threading.Tasks;
using System;
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
                this.TeacherService.RetrieveTeacherByIdAsync(someId);

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
    }
}
