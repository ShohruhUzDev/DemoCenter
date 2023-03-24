using System;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someSubjectId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedSubjectException =
                new LockedSubjectException(databaseUpdateConcurrencyException);

            var expectedSubjectDependencyValidationException =
                new SubjectDependencyValidationException(lockedSubjectException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Subject> removeSubjectByIdTask =
                this.subjectService.RemoveSubjectByIdAsync(someSubjectId);

            SubjectDependencyValidationException actualSubjectDependencyValidationException =
                await Assert.ThrowsAsync<SubjectDependencyValidationException>(
                    removeSubjectByIdTask.AsTask);

            // then
            actualSubjectDependencyValidationException.Should().BeEquivalentTo(
                expectedSubjectDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
      
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnDeleteWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someSubjectId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedSubjectStorageException =
                new FailedSubjectStorageException(sqlException);

            var expectedSubjectDependencyException =
                new SubjectDependencyException(failedSubjectStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Subject> deleteSubjectTask =
                this.subjectService.RemoveSubjectByIdAsync(someSubjectId);

            SubjectDependencyException actualSubjectDependencyException =
                await Assert.ThrowsAsync<SubjectDependencyException>(
                    deleteSubjectTask.AsTask);

            // then
            actualSubjectDependencyException.Should().BeEquivalentTo(
                expectedSubjectDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSubjectDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someSubjectId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedSubjectServiceException =
                new FailedSubjectServiceException(serviceException);

            var expectedSubjectServiceException =
                new SubjectServiceException(failedSubjectServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Subject> removeSubjectByIdTask =
                this.subjectService.RemoveSubjectByIdAsync(someSubjectId);

            SubjectServiceException actualSubjectServiceException =
                await Assert.ThrowsAsync<SubjectServiceException>(
                    removeSubjectByIdTask.AsTask);

            // then
            actualSubjectServiceException.Should().BeEquivalentTo(
                expectedSubjectServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
