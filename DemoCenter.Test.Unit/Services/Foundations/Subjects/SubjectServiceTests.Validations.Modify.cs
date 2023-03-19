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
        public async Task ShouldThrowValidationExceptionOnModifyIfSubjectIsNullAndLogItAsync()
        {
            //given
            Subject nullSubject = null;
            var nullSubjectException = new NullSubjectException();

            var expectedSubjectValidationException = new SubjectValidationException(nullSubjectException);

            //when
            ValueTask<Subject> onModifySubjectTask = this.subjectService.ModifySubjectAsync(nullSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(onModifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should().BeEquivalentTo(expectedSubjectValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfSubjectIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            var invalidSubject = new Subject
            {
                SubjectName = invalidString
            };

            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.Id),
                values: "Id is required");

            invalidSubjectException.AddData(
                key: nameof(Subject.SubjectName),
                values: "Text is required");

            invalidSubjectException.AddData(
                key: nameof(Subject.Price),
                values: "Value is required");


            invalidSubjectException.AddData(
                key: nameof(Subject.CreatedDate),
                values: "Value is required");

            invalidSubjectException.AddData(
                key: nameof(Subject.UpdatedDate),
                values: new[]
                {
                    "Value is required",
                    "Date is not recent",
                    $"Date is the same as {nameof(Subject.CreatedDate)}"
                });

            var expectedSubjectValidationException=
                new SubjectValidationException(invalidSubjectException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Returns(GetRandomDateTimeOffset);

            //when
            ValueTask<Subject> onModifySubjectTask = 
                this.subjectService.ModifySubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(onModifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrenDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))), Times.Once());   

            this.storageBrokerMock.Verify(broker=>
                broker.UpdateSubjectAsync(It.IsAny<Subject>()), Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdateDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime=GetRandomDateTimeOffset();
            Subject randomSubect = CreateRandomSubject(randomDateTime);
            Subject invalidSubject = randomSubect;
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key:nameof(Subject.UpdatedDate),
                values:$"Date is the same as {nameof(Subject.CreatedDate)}");   

            var expectedSubjectValidationException=
                new SubjectValidationException(invalidSubjectException);    

            this.dateTimeBrokerMock.Setup(broker=>
                broker.GetCurrenDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Subject> modifySubjectTask = 
                this.subjectService.ModifySubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                 await Assert.ThrowsAsync<SubjectValidationException>(modifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectSubjectByIdAsync(invalidSubject.Id), Times.Never());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(expectedSubjectValidationException))), Times.Once()); 

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpDatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            //given
            DateTimeOffset randomDateTime=GetRandomDateTimeOffset();
            Subject randomSubject = CreateRandomSubject(randomDateTime);
            Subject inputSubject = randomSubject;
            inputSubject.UpdatedDate=randomDateTime.AddMinutes(invalidSeconds);
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.UpdatedDate),
                values: "Date is not recent");
            
            var expectedSubjectValidationException=
                new SubjectValidationException(invalidSubjectException);    
           
            this.dateTimeBrokerMock.Setup(broker=>
                broker.GetCurrenDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Subject> modifySubjectTask = 
                this.subjectService.ModifySubjectAsync(inputSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(modifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(inputSubject.Id), Times.Never);

            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))),Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
