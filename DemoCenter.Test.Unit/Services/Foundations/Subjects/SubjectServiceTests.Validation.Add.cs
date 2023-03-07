using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            //given
            Subject noSubject = null;
            var nullSubjectException = new NullSubjectException();
            
            var expectedSubjectValidationException=
                new SubjectValidationException(nullSubjectException);

            //when
            ValueTask<Subject> addSubjectTask=
                this.subjectService.AddSubjectAsync(noSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(addSubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should().
                BeEquivalentTo(expectedSubjectValidationException);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))), Times.Once());   

            this.storageBrokerMock.Verify(broker=>
                broker.InsertSubjectAsync(It.IsAny<Subject>()),Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
             
        }
    }
}
