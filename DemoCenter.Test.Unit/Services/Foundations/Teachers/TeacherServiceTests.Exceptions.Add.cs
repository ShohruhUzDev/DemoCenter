using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Teachers;
using DemoCenter.Models.Foundations.Teachers.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Teacher someTeacher = CreateRandomTeacher();
            SqlException sqlException = CreateSqlException();
            var failedTeacherStorageException = new FailedTeacherStorageException(sqlException);

            var expectedTeacherDependencyException =
                new TeacherDependencyException(failedTeacherStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Throws(sqlException);

            //when
            ValueTask<Teacher> addTeacherTask = this.teacherService.AddTeacherAsync(someTeacher);

            TeacherDependencyException actualTeacherDependencyException =
                await Assert.ThrowsAsync<TeacherDependencyException>(addTeacherTask.AsTask);

            //then
            actualTeacherDependencyException.Should()
                .BeEquivalentTo(expectedTeacherDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTeacherDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldTrowDependencyValidationExceptionOnAddIfDuplicateErrorOccursAndLogItAsync()
        {
            // given
            Teacher someTeacher = CreateRandomTeacher();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsTeacherException =
                new AlreadyExistTeacherException(duplicateKeyException);

            var expectedTeacherDependencyValidationException =
                new TeacherDependencyValidationException(alreadyExistsTeacherException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTime())
                .Throws(duplicateKeyException);

            // when
            ValueTask<Teacher> addTeacherTask = this.teacherService.AddTeacherAsync(someTeacher);

            TeacherDependencyValidationException actualTeacherDependencyValidationException =
                await Assert.ThrowsAsync<TeacherDependencyValidationException>(addTeacherTask.AsTask);

            // then
            actualTeacherDependencyValidationException.Should().BeEquivalentTo(
                expectedTeacherDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptionAs(
                expectedTeacherDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker => broker.InsertTeacherAsync(
               It.IsAny<Teacher>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDbConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Teacher someTeacher = CreateRandomTeacher();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedTeacherException = new LockedTeacherException(dbUpdateConcurrencyException);

            var expectedTeacherDependencyValidationException =
                new TeacherDependencyValidationException(lockedTeacherException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Throws(dbUpdateConcurrencyException);

            // when
            ValueTask<Teacher> addTeacherTask = this.teacherService.AddTeacherAsync(someTeacher);

            TeacherDependencyValidationException actualTeacherDependencyValidationException =
                 await Assert.ThrowsAsync<TeacherDependencyValidationException>(addTeacherTask.AsTask);

            // then
            actualTeacherDependencyValidationException.Should()
                .BeEquivalentTo(expectedTeacherDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                SameExceptionAs(expectedTeacherDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Teacher someTeacher = CreateRandomTeacher();
            var serviceException = new Exception();

            var failedTeacherServiceException =
                new FailedTeacherServiceException(serviceException);

            var expectedTeacherServiceException =
                new TeacherServiceException(failedTeacherServiceException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTime())
                .Throws(serviceException);

            //when
            ValueTask<Teacher> addTeacherTask =
                this.teacherService.AddTeacherAsync(someTeacher);

            TeacherServiceException actualTeacherServiceException =
                await Assert.ThrowsAsync<TeacherServiceException>(addTeacherTask.AsTask);

            //then
            actualTeacherServiceException.Should().BeEquivalentTo(expectedTeacherServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherServiceException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
