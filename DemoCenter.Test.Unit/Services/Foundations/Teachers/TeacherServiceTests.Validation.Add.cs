using System;
using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            //given
            Teacher noTeacher = null;
            var nullTeacherException = new NullTeacherException();

            var expectedTeacherValidationException =
                new TeacherValidationException(nullTeacherException);

            //when
            ValueTask<Teacher> addTeacherTask = this.teacherService.AddTeacherAsync(noTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(addTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should().
                BeEquivalentTo(expectedTeacherValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfInputInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            var invalidTeacher = new Teacher
            {
                FirstName = invalidString,
                LastName = invalidString,
                Phone = invalidString

            };

            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key:nameof(Teacher.Id),
                values:"Id is required");

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
                values: "Value is required");

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacherException);

            //when
            ValueTask<Teacher> addTeacherTask = 
                 this.teacherService.AddTeacherAsync(invalidTeacher);

            TeacherValidationException actualTeacherValidationException=
                await Assert.ThrowsAsync<TeacherValidationException>(addTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker=>
                broker.InsertTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameUpdatedDateAndLogItAsync()
        {
            //given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Teacher randomTeacher = CreateRandomTeacher(randomDate);
            Teacher invalidTeacher = randomTeacher;
            invalidTeacher.UpdatedDate=randomDate.AddMinutes(randomMinutes);
            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key: nameof(Teacher.CreatedDate),
                values: $"Date is not same as {nameof(Teacher.UpdatedDate)}");

            var expectedTeacherValidationException = new TeacherValidationException(invalidTeacherException);

            //when
            ValueTask<Teacher> addTeacherTask = this.teacherService.AddTeacherAsync(invalidTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(addTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should().BeEquivalentTo(expectedTeacherValidationException);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker=>
                broker.InsertTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();

        }

    }
}