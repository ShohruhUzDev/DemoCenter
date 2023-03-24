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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            //given
            DateTimeOffset randomDate=GetRandomDateTime();
            Subject randomSubject = CreateRandomSubject(randomDate);
            Subject someSubject = randomSubject;
            Guid someId=someSubject.Id;
            SqlException sqlException = CreateSqlException();

            var failedSubjectStorageException =
                new FailedSubjectStorageException(sqlException);

            var expectedSubjectDependencyException =
                new SubjectDependencyException(failedSubjectStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Throws(sqlException);

            //when
            ValueTask<Subject> modifySubjectTask=
                this.subjectService.ModifySubjectAsync(someSubject);

            SubjectDependencyException actualSubjectDependencyException =
                await Assert.ThrowsAsync<SubjectDependencyException>(modifySubjectTask.AsTask);

            //then
            actualSubjectDependencyException.Should()
                .BeEquivalentTo(expectedSubjectDependencyException);

            this.dateTimeBrokerMock.Verify(Brokers =>
                Brokers.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSubjectDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectSubjectByIdAsync(someId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubjectAsync(someSubject), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();

        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            //given
            int minutesInPas = GetRandomNegativeNumber();
            DateTimeOffset randomDate = GetRandomDateTime();
            Subject randomSubject = CreateRandomSubject(randomDate);
            Subject someSubject = randomSubject;
            Guid someId = someSubject.Id;
            someSubject.CreatedDate = randomDate.AddMinutes(minutesInPas);
            var databaseUpdateException = new DbUpdateException();

            var failedSubjectStorageException=
                new FailedSubjectStorageException(databaseUpdateException);

            var expectedSubjectDependencyException =
                new SubjectDependencyException(failedSubjectStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(someId)).ThrowsAsync(databaseUpdateException);

            this.dateTimeBrokerMock.Setup(broker=>
                broker.GetCurrentDateTime()).Returns(randomDate);

            //when
            ValueTask<Subject> modifySubjectTask =
                this.subjectService.ModifySubjectAsync(someSubject);

            SubjectDependencyException actualSubjectDependencyException =
                await Assert.ThrowsAsync<SubjectDependencyException>(modifySubjectTask.AsTask);

            //then
            actualSubjectDependencyException.Should()
                .BeEquivalentTo(expectedSubjectDependencyException);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectSubjectByIdAsync(someId), Times.Once());   

            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectDependencyException))), Times.Once);

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
            Subject randomSubject = CreateRandomSubject(randomDateTime);
            Subject someSubject = randomSubject;
            someSubject.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            Guid SubjectId = someSubject.Id;
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedSubjectException =
                new LockedSubjectException(databaseUpdateConcurrencyException);

            var expectedSubjectDependencyValidationException =
                new SubjectDependencyValidationException(lockedSubjectException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(SubjectId))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            // when
            ValueTask<Subject> modifySubjectTask =
                this.subjectService.ModifySubjectAsync(someSubject);

            SubjectDependencyValidationException actualSubjectDependencyValidationException =
                await Assert.ThrowsAsync<SubjectDependencyValidationException>(
                    modifySubjectTask.AsTask);

            // then
            actualSubjectDependencyValidationException.Should().BeEquivalentTo(
                expectedSubjectDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(SubjectId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectDependencyValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
