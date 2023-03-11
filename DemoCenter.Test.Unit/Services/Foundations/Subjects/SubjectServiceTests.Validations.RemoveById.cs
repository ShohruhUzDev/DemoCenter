using System;
using System.Threading.Tasks;
using DemoCenter.Models.Students.Exceptions;
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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidSubjectId = Guid.Empty;
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.Id),
                values: "Id is required");

            var expectedSubjectValidationException = 
                new SubjectValidationException(invalidSubjectException);

            //when
            ValueTask<Subject> onRemoveSubjectTask=this.subjectService
                .RemoveSubjectByIdAsync(invalidSubjectId);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(onRemoveSubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfStudentIsNotFoundAndLogItAsync()
        {
            //given
            Guid randomSubjectId= Guid.NewGuid();
            Guid inputSubjectId = randomSubjectId;
            Subject noSubject = null;
            var notFoundSubjectException = new NotFoundSubjectException(inputSubjectId);

            var expectedSubjectValidationException =
                new SubjectValidationException(notFoundSubjectException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noSubject);
            
            //when
            ValueTask<Subject> onRemoveSubjectTask=this.subjectService.RemoveSubjectByIdAsync(inputSubjectId);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(onRemoveSubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectSubjectByIdAsync(It.IsAny<Guid>()), Times.Once());

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
