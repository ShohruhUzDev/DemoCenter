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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTimeOffset();
            Teacher randomTeacher = CreateRandomTeacher(someDateTime);
            Teacher someTeacher = randomTeacher;
            Guid TeacherId = someTeacher.Id;
            SqlException sqlException = CreateSqlException();

            var failedTeacherStorageException =
                new FailedTeacherStorageException(sqlException);

            var expectedTeacherDependencyException =
                new TeacherDependencyException(failedTeacherStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Throws(sqlException);

            // when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(someTeacher);

            TeacherDependencyException actualTeacherDependencyException =
                await Assert.ThrowsAsync<TeacherDependencyException>(
                     modifyTeacherTask.AsTask);

            // then
            actualTeacherDependencyException.Should().BeEquivalentTo(
                expectedTeacherDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTeacherDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(TeacherId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTeacherAsync(someTeacher), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
