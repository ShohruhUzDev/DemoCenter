using System;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects.Exceptions;
using DemoCenter.Models.Subjects;
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
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            //given
            Subject noSubject = null;
            var nullSubjectException = new NullSubjectException();

            var expectedSubjectValidationException =
                new SubjectValidationException(nullSubjectException);

            //when
            ValueTask<Subject> addSubjectTask =
                this.subjectService.AddSubjectAsync(noSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(addSubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should().
                BeEquivalentTo(expectedSubjectValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsInvalidAndLogItAsync(
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
                values: "Value is required");

            var expectedSubjetValidationException =
                new SubjectValidationException(invalidSubjectException);

            //when
            ValueTask<Subject> addSubjectTask = this.subjectService.AddSubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(addSubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjetValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjetValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedTimeIsNotSameUpdatedTimeAndLogIstAsync()
        {
            //given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Subject randomSubject = CreateRandomSubject(randomDate);
            Subject invalidSubject = randomSubject;
            invalidSubject.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.CreatedDate),
                values: $"Date is not same as {nameof(Subject.UpdatedDate)}");

            var expectedSubjectValidationExeption = new SubjectValidationException(invalidSubjectException);

            //when
            ValueTask<Subject> addSubjectTask = this.subjectService.AddSubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(addSubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should().BeEquivalentTo(expectedSubjectValidationExeption);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedSubjectValidationExeption))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
                  int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset invalidRandomDateTime = randomDateTime.AddSeconds(invalidSeconds);
            Subject randomInvalidSubject = CreateRandomSubject(invalidRandomDateTime);
            Subject invalidSubject = randomInvalidSubject;
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.CreatedDate),
                values: "Date is not recent");

            var expectedSubjectValidationException =
                new SubjectValidationException(invalidSubjectException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Subject> addSubjectTask = this.subjectService.AddSubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(addSubjectTask.AsTask);

            // then
            actualSubjectValidationException.Should().BeEquivalentTo(
                expectedSubjectValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubjectAsync(It.IsAny<Subject>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
