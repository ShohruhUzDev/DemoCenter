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
    }
}
