using DemoCenter.Models.Subjects.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using System;
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
    }
}
