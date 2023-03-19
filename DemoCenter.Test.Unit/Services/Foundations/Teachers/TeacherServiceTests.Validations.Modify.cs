using System;
using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
                FirstName = invalidString,
                LastName = invalidString,
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

            var expectedTeacherValidationException=
                new TeacherValidationException(invalidTeacherException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Returns(GetRandomDateTimeOffset);

            //when
            ValueTask<Teacher> modifyTeacherTask =
                this.teacherService.ModifyTeacherAsync(invalidTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(modifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationException))), Times.Once);   

            this.storageBrokerMock.Verify(broker=>
                broker.UpdateTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]  
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime=GetRandomDateTimeOffset   ();
            Teacher randomTeacher = CreateRandomTeacher(randomDateTime);
            Teacher invalidTeacher = randomTeacher;
            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key:nameof(Teacher.UpdatedDate),
                values:$"Date is the same as {nameof(Teacher.CreatedDate)}");

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacherException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Returns(randomDateTime);

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
                broker.GetCurrenDateTime(), Times.Once);

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
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Teacher randomTeacher = CreateRandomTeacher(randomDateTime);
            Teacher inputTeacher = randomTeacher;
            inputTeacher.UpdatedDate=randomDateTime.AddMinutes(invalidSeconds);
            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key: nameof(Teacher.UpdatedDate),
                values: "Date is not recent");

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacherException);

            this.dateTimeBrokerMock.Setup(broker=>
                broker.GetCurrenDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Teacher> modifyTeacherTask=this.teacherService.ModifyTeacherAsync(inputTeacher);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(modifyTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
