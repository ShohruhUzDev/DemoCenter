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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedSubjectStorageException=new FailedSubjectStorageException(sqlException);

            var expectedSubjectDependencyException =
                new SubjectDependencyException(failedSubjectStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()))
                     .ThrowsAsync(sqlException);

            //when
            ValueTask<Subject> retrieveSubjectTask=
                this.subjectService.RetrieveSubjectByIdAsync(someId);

            SubjectDependencyException actualSubjectDependencyException =
                await Assert.ThrowsAsync<SubjectDependencyException>(retrieveSubjectTask.AsTask);

            //then
            actualSubjectDependencyException.Should()
                .BeEquivalentTo(expectedSubjectDependencyException);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()), Times.Once()); 

            this.loggingBrokerMock.Verify(broker=>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSubjectDependencyException))),Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls(); 
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedSubjectServiceException =
                new FailedSubjectServiceException(serviceException);

            var expectedSubjectServiceExcpetion =
                new SubjectServiceException(failedSubjectServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            // when
            ValueTask<Subject> retrieveSubjectById =
            this.SubjectService.RetrieveSubjectByIdAsync(someId);

            SubjectServiceException actualSubjectServiceException =
                await Assert.ThrowsAsync<SubjectServiceException>(retrieveSubjectById.AsTask);

            // then
            actualSubjectServiceException.Should().BeEquivalentTo(expectedSubjectServiceExcpetion);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedSubjectServiceExcpetion))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
