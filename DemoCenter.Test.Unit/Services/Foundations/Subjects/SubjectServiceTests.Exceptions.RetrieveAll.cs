using System;
using DemoCenter.Models.Foundations.Subjects.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogIt()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedSubjectStorageException =
                new FailedSubjectStorageException(sqlException);

            var expectedSubjectDependencyException =
                new SubjectDependencyException(failedSubjectStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSubjects()).Throws(sqlException);

            // when
            Action retrieveAllSubjectsAction = () =>
                this.subjectService.RetrieveAllSubjects();

            SubjectDependencyException actualSubjectDependencyException =
                Assert.Throws<SubjectDependencyException>(retrieveAllSubjectsAction);

            // then
            actualSubjectDependencyException.Should().BeEquivalentTo(expectedSubjectDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSubjects(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSubjectDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllWhenAllServiceErrorOccursAndLogIt()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedSubjectServiceException =
                new FailedSubjectServiceException(serviceException);

            var expectedSubjectServiceException =
                new SubjectServiceException(failedSubjectServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSubjects()).Throws(serviceException);

            // when
            Action retrieveAllSubjectAction = () =>
                this.subjectService.RetrieveAllSubjects();

            SubjectServiceException actualSubjectServiceException =
                Assert.Throws<SubjectServiceException>(retrieveAllSubjectAction);

            // then
            actualSubjectServiceException.Should().BeEquivalentTo(expectedSubjectServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSubjects(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
