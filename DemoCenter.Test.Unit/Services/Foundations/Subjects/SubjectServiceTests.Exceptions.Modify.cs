using System;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
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
    }
}
