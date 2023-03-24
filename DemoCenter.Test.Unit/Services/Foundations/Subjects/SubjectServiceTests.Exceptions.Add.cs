using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldTrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Subject someSubject=CreateRandomSubject();
            SqlException sqlException = CreateSqlException();
            var failedSubjectStorageException = new FailedSubjectStorageException(sqlException);

            var expectedSubjectDependencyException =
                new SubjectDependencyException(failedSubjectStorageException);

            this.dateTimeBrokerMock.Setup(broker=>
                broker.GetCurrentDateTime()).Throws(sqlException);

            //when
            ValueTask<Subject> addSubjectTask=this.subjectService.AddSubjectAsync(someSubject);

            SubjectDependencyException actualSubjectDependencyException =
                await Assert.ThrowsAsync<SubjectDependencyException>(addSubjectTask.AsTask);

            //then
            actualSubjectDependencyException.Should()
                .BeEquivalentTo(expectedSubjectDependencyException); 
            
            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSubjectDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateErrorOccursAndLogItAsync()
        {
            //given
            Subject someSubject = CreateRandomSubject();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsSubjectException =
                new AlreadyExistsSubjectException(duplicateKeyException);

            var expextedSubjectDependencyValidationException=
                new SubjectDependencyValidationException(alreadyExistsSubjectException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Throws(duplicateKeyException);

            //when
            ValueTask<Subject> addSubjectTask = this.subjectService.AddSubjectAsync(someSubject);

            SubjectDependencyValidationException actualSubjectDependencyValidationException =
                await Assert.ThrowsAsync<SubjectDependencyValidationException>(addSubjectTask.AsTask);

            //then
            actualSubjectDependencyValidationException.Should()
                .BeEquivalentTo(expextedSubjectDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expextedSubjectDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDbConcurrencyErrorOccursAndLogItAsync()
        {
            //given
            Subject someSubject = CreateRandomSubject();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();
            var lockedSubjectException = new LockedSubjectException(dbUpdateConcurrencyException);

            var expectedSubjectDependencyValidationException =
                new SubjectDependencyValidationException(lockedSubjectException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Throws(dbUpdateConcurrencyException);

            //when
            ValueTask<Subject> addSubjectTask=this.subjectService.AddSubjectAsync(someSubject);

            SubjectDependencyValidationException actualSubjectDependencyValidationException =
                await Assert.ThrowsAsync<SubjectDependencyValidationException>(addSubjectTask.AsTask);

            //then
            actualSubjectDependencyValidationException.Should()
                .BeEquivalentTo(expectedSubjectDependencyValidationException);   

            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
