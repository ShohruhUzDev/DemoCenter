using System.Threading.Tasks;
using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Student someStudent = CreateRandomStudent();
            SqlException sqlException = CreateSqlException();
            var failedStudentStorageException = new FailedStudentStorageException(sqlException);

            var expectedStudentDependencyException =
                new StudentDependencyException(failedStudentStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Throws(sqlException);

            // when
            ValueTask<Student> addStudentTask = this.studentService.AddStudentAsync(someStudent);

            StudentDependencyException actualStudentDependencyException =
                await Assert.ThrowsAsync<StudentDependencyException>(addStudentTask.AsTask);

            // then
            actualStudentDependencyException.Should().BeEquivalentTo(expectedStudentDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedStudentDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStudentAsync(It.IsAny<Student>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldTrowDependencyValidationExceptionOnAddIfDuplicateErrorOccursAndLogItAsync()
        {
            // given
            Student someStudent = CreateRandomStudent();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsStudentException =
                new AlreadyExistsStudentException(duplicateKeyException);

            var expectedStudentDependencyValidationException =
                new StudentDependencyValidationException(alreadyExistsStudentException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTime())
                .Throws(duplicateKeyException);

            // when
            ValueTask<Student> addStudentTask = this.studentService.AddStudentAsync(someStudent);

            StudentDependencyValidationException actualStudentDependencyValidationException =
                await Assert.ThrowsAsync<StudentDependencyValidationException>(addStudentTask.AsTask);

            // then
            actualStudentDependencyValidationException.Should().BeEquivalentTo(
                expectedStudentDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptionAs(
                expectedStudentDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker => broker.InsertStudentAsync(
               It.IsAny<Student>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDbConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Student someStudent = CreateRandomStudent();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();
            var lockedStudentException = new LockedStudentException(dbUpdateConcurrencyException);

            var expectedStudentDependencyValidationException =
                new StudentDependencyValidationException(lockedStudentException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTime())
                .Throws(dbUpdateConcurrencyException);

            // when
            ValueTask<Student> addStudentTask = this.StudentService.AddStudentAsync(someStudent);

            StudentDependencyValidationException actualStudentDependencyValidationException =
                await Assert.ThrowsAsync<StudentDependencyValidationException>(addStudentTask.AsTask);

            // then
            actualStudentDependencyValidationException.Should()
                .BeEquivalentTo(expectedStudentDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedStudentDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStudentAsync(It.IsAny<Student>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
