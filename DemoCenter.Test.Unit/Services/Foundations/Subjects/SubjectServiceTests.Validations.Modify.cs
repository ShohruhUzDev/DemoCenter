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
        public async Task ShouldThrowValidationExceptionOnModifyIfSubjectIsNullAndLogItAsync()
        {
            //given
            Subject nullSubject = null;
            var nullSubjectException=new NullSubjectException();    

            var expectedSubjectValidationException=new SubjectValidationException(nullSubjectException);

            //when
            ValueTask<Subject> onModifySubjectTask=this.subjectService.ModifySubjectAsync(nullSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(onModifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should().BeEquivalentTo(expectedSubjectValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker=>
                broker.UpdateSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
