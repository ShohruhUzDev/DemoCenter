using System;
using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
            DateTimeOffset someDateTime = GetRandomDateTime();
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
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomTeacher(randomDateTime);
            Teacher someTeacher = randomTeacher;
            Guid TeacherId = someTeacher.Id;
            someTeacher.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedTeacherException =
                new FailedTeacherStorageException(databaseUpdateException);

            var expectedTeacherDependencyException =
                new TeacherDependencyException(failedTeacherException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(TeacherId))
                    .ThrowsAsync(databaseUpdateException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(someTeacher);

            TeacherDependencyException actualTeacherDependencyException =
                 await Assert.ThrowsAsync<TeacherDependencyException>(
                     modifyTeacherTask.AsTask);

            // then
            actualTeacherDependencyException.Should().BeEquivalentTo(
                expectedTeacherDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(TeacherId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomTeacher(randomDateTime);
            Teacher someTeacher = randomTeacher;
            someTeacher.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            Guid TeacherId = someTeacher.Id;
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedTeacherException =
                new LockedTeacherException(databaseUpdateConcurrencyException);

            var expectedTeacherDependencyValidationException =
                new TeacherDependencyValidationException(lockedTeacherException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(TeacherId))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(someTeacher);

            TeacherDependencyValidationException actualTeacherDependencyValidationException =
                await Assert.ThrowsAsync<TeacherDependencyValidationException>(
                    modifyTeacherTask.AsTask);

            // then
            actualTeacherDependencyValidationException.Should().BeEquivalentTo(
                expectedTeacherDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(TeacherId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            int minuteInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomTeacher(randomDateTime);
            Teacher someTeacher = randomTeacher;
            someTeacher.CreatedDate = randomDateTime.AddMinutes(minuteInPast);
            var serviceException = new Exception();

            var failedTeacherException =
                new FailedTeacherServiceException(serviceException);

            var expectedTeacherServiceException =
                new TeacherServiceException(failedTeacherException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(someTeacher.Id))
                    .ThrowsAsync(serviceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Teacher> modifyTeacherTask =
                this.TeacherService.ModifyTeacherAsync(someTeacher);

            TeacherServiceException actualTeacherServiceException =
                await Assert.ThrowsAsync<TeacherServiceException>(
                    modifyTeacherTask.AsTask);

            // then
            actualTeacherServiceException.Should().BeEquivalentTo(
                expectedTeacherServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(someTeacher.Id), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
