using System;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTests
    {
        [Fact]
        public async Task ShouldThrowVaidationExceptionOnRetrieveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidSubjectId = Guid.Empty;
            var invalidSubjectException = new InvalidSubjectException();

            var expectedSubjectValidationException = 
                new SubjectValidationException(invalidSubjectException);

            //when
            ValueTask<Subject> onRetrieveSubjectTask=
                this.subjectService.RetrieveSubjectByIdAsync(invalidSubjectId);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(onRetrieveSubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);   

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()), Times.Never);
            
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}