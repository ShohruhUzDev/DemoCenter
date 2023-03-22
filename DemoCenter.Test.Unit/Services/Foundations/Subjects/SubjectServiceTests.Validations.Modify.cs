using System;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
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

            var expectedSubjectValidationException =
                new SubjectValidationException(invalidSubjectException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(GetRandomDateTime);

            //when
            ValueTask<Subject> onModifySubjectTask =
                this.subjectService.ModifySubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(onModifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubjectAsync(It.IsAny<Subject>()), Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdateDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Subject randomSubect = CreateRandomSubject(randomDateTime);
            Subject invalidSubject = randomSubect;
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.UpdatedDate),
                values: $"Date is the same as {nameof(Subject.CreatedDate)}");

            var expectedSubjectValidationException =
                new SubjectValidationException(invalidSubjectException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Subject> modifySubjectTask =
                this.subjectService.ModifySubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                 await Assert.ThrowsAsync<SubjectValidationException>(modifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(invalidSubject.Id), Times.Never());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
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
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Subject randomSubject = CreateRandomSubject(randomDateTime);
            Subject inputSubject = randomSubject;
            inputSubject.UpdatedDate = randomDateTime.AddMinutes(invalidSeconds);
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.UpdatedDate),
                values: "Date is not recent");

            var expectedSubjectValidationException =
                new SubjectValidationException(invalidSubjectException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfSubjectDoesNotExistAndLogItAsync()
        {
            //given
            int randomNegativNumbers = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Subject randomSubject = CreateRandomSubject(randomDateTime);
            Subject nonExistSubject = randomSubject;
            nonExistSubject.CreatedDate = randomDateTime.AddMinutes(randomNegativNumbers);
            Subject nullSubject = null;

            var notFoundSubjectException =
                new NotFoundSubjectException(nonExistSubject.Id);

            var expectedSubjectValidationException =
                new SubjectValidationException(notFoundSubjectException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(nonExistSubject.Id)).ReturnsAsync(nullSubject);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Subject> modifySubject =
                this.subjectService.ModifySubjectAsync(nonExistSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(modifySubject.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(nonExistSubject.Id), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItASync()
        {
            //given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Subject randomSubject = CreateRandomModifySubjects(randomDateTime);
            Subject invalidSubject = randomSubject.DeepClone();
            Subject storageSubject = randomSubject.DeepClone();
            storageSubject.CreatedDate = storageSubject.CreatedDate.AddMinutes(randomMinutes);
            storageSubject.UpdatedDate = storageSubject.UpdatedDate.AddMinutes(randomMinutes);
            Guid subjectId = invalidSubject.Id;
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.CreatedDate),
                values: $"Date is not same as {nameof(Subject.CreatedDate)}");

            var expectedSubjectValidationException =
                new SubjectValidationException(invalidSubjectException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(subjectId)).ReturnsAsync(storageSubject);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Subject> modifySubjectTask =
                this.subjectService.ModifySubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(modifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(subjectId), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Subject randomSubject = CreateRandomModifySubjects(randomDateTime);
            Subject invalidSubject = randomSubject;
            Subject storageSubject = randomSubject.DeepClone();
            invalidSubject.UpdatedDate = storageSubject.UpdatedDate;
            Guid subjectId = invalidSubject.Id;
            var invalidSubjectException = new InvalidSubjectException();

            invalidSubjectException.AddData(
                key: nameof(Subject.UpdatedDate),
                values: $"Date is the same as {nameof(Subject.UpdatedDate)}");

            var expectedSubjectValidationException =
                new SubjectValidationException(invalidSubjectException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(invalidSubject.Id)).ReturnsAsync(storageSubject);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Subject> modifySubjectTask =
                this.subjectService.ModifySubjectAsync(invalidSubject);

            SubjectValidationException actualSubjectValidationException =
                await Assert.ThrowsAsync<SubjectValidationException>(modifySubjectTask.AsTask);

            //then
            actualSubjectValidationException.Should()
                .BeEquivalentTo(expectedSubjectValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(subjectId), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubjectValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
