using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using System;
using Xunit;
using DemoCenter.Models.Students.Exceptions;
using DemoCenter.Models.Students;
using FluentAssertions;
using Microsoft.Data.SqlClient;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someStudentId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedStudentException =
                new LockedStudentException(databaseUpdateConcurrencyException);

            var expectedStudentDependencyValidationException =
                new StudentDependencyValidationException(lockedStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Student> removeStudentByIdTask =
                this.studentService.RemoveStudentByIdAsync(someStudentId);

            StudentDependencyValidationException actualStudentDependencyValidationException =
                await Assert.ThrowsAsync<StudentDependencyValidationException>(
                    removeStudentByIdTask.AsTask);

            // then
            actualStudentDependencyValidationException.Should().BeEquivalentTo(
                expectedStudentDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteStudentAsync(It.IsAny<Student>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnDeleteWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someStudentId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedStudentStorageException =
                new FailedStudentStorageException(sqlException);

            var expectedStudentDependencyException =
                new StudentDependencyException(failedStudentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Student> deleteStudentTask =
                this.studentService.RemoveStudentByIdAsync(someStudentId);

            StudentDependencyException actualStudentDependencyException =
                await Assert.ThrowsAsync<StudentDependencyException>(
                    deleteStudentTask.AsTask);

            // then
            actualStudentDependencyException.Should().BeEquivalentTo(
                expectedStudentDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedStudentDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
