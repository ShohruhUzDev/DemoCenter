using System;
using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfTeacherIsNullAndLogItAsync()
        {
            //given
            Teacher nullTeacher = null;
            var nullTeacherException = new NullTeacherException();

            var expectedTeacherValidationException = new TeacherValidationException(nullTeacherException);

            //when
            ValueTask<Teacher> onModifyTeacherTask = this.teacherService.ModifyTeacherAsync(nullTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(onModifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should().BeEquivalentTo(expectedTeacherValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfTeacherIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            var invalidTeacher = new Teacher
            {
                FirstName = invalidString
              
            };

            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key: nameof(Teacher.Id),
                values: "Id is required");

            invalidTeacherException.AddData(
                key: nameof(Teacher.FirstName),
                values: "Text is required");

            invalidTeacherException.AddData(
                key: nameof(Teacher.LastName),
                values: "Text is required");

            invalidTeacherException.AddData(
                key: nameof(Teacher.Phone),
                values: "Text is required");

            invalidTeacherException.AddData(
                key: nameof(Teacher.CreatedDate),
                values: "Value is required");

            invalidTeacherException.AddData(
              key: nameof(Teacher.UpdatedDate),
              values: new[]
              {
                  "Value is required",
                  "Date is not recent",
                  $"Date is the same as {nameof(Teacher.CreatedDate)}"
              });

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacherException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(GetRandomDateTime);

            //when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(invalidTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(modifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomTeacher(randomDateTime);
            Teacher invalidTeacher = randomTeacher;
            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key: nameof(Teacher.UpdatedDate),
                values: $"Date is the same as {nameof(Teacher.CreatedDate)}");

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacherException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(invalidTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(modifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                 .BeEquivalentTo(expectedTeacherValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(invalidTeacher.Id), Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomTeacher(randomDateTime);
            Teacher inputTeacher = randomTeacher;
            inputTeacher.UpdatedDate = randomDateTime.AddMinutes(invalidSeconds);
            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key: nameof(Teacher.UpdatedDate),
                values: "Date is not recent");

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacherException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Teacher> modifyTeacherTask = this.teacherService.ModifyTeacherAsync(inputTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(modifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfTeacherDoesNotExistAndLogItAsync()
        {
            //given
            int randomNegativeNumbers = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomTeacher(randomDateTime);
            Teacher nonExistTeacher = randomTeacher;
            nonExistTeacher.CreatedDate = randomDateTime.AddMinutes(randomNegativeNumbers);
            Teacher nullTeacher = null;

            var notFounTeacherException = new NotFoundTeacherException(nonExistTeacher.Id);

            var expectedTeacherValidationException =
                new TeacherValidationException(notFounTeacherException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(nonExistTeacher.Id)).ReturnsAsync(nullTeacher);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(nonExistTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(modifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(nonExistTeacher.Id), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            //given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomModifyTeacher(randomDateTime);
            Teacher invalidTeacher = randomTeacher.DeepClone();
            Teacher storageTeacher = invalidTeacher.DeepClone();
            storageTeacher.CreatedDate = storageTeacher.CreatedDate.AddMinutes(randomMinutes);
            storageTeacher.UpdatedDate = storageTeacher.UpdatedDate.AddMinutes(randomMinutes);
            Guid teacherId = invalidTeacher.Id;
            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key: nameof(Teacher.CreatedDate),
                values: $"Date is not same as {nameof(Teacher.CreatedDate)}");

            var expectedTeacherValidationExcepiton =
                new TeacherValidationException(invalidTeacherException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(teacherId)).ReturnsAsync(storageTeacher);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(invalidTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(modifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationExcepiton);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(teacherId), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationExcepiton))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomModifyTeacher(randomDateTime);
            Teacher invalidTeacher = randomTeacher;
            Teacher storageTeacher = randomTeacher.DeepClone();
            invalidTeacher.UpdatedDate = storageTeacher.UpdatedDate;
            Guid teacherId = invalidTeacher.Id;
            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key: nameof(Teacher.UpdatedDate),
                values: $"Date is the same as {nameof(Teacher.UpdatedDate)}");

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacherException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(teacherId)).ReturnsAsync(storageTeacher);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(invalidTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(modifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(teacherId), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationException))), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
