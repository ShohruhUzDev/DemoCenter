using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Teachers;
using DemoCenter.Models.Foundations.Teachers.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidTeacherId = Guid.Empty;
            var invalidTeacheException = new InvalidTeacherException();

            invalidTeacheException.AddData(
                key: nameof(Teacher.Id),
                values: "Id is required");

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacheException);

            //when
            ValueTask<Teacher> onRemoveTeacherTask = this.teacherService.RemoveTeacherByIdAsync(invalidTeacherId);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(onRemoveTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfTeacherNotFoundAndLogItAsync()
        {
            //given
            Guid randomTeacherId = Guid.NewGuid();
            Guid inputTeacherId = randomTeacherId;
            Teacher noTeacher = null;
            var notFoundTeacherException = new NotFoundTeacherException(inputTeacherId);

            var expectedTeacherValidationException =
                new TeacherValidationException(notFoundTeacherException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noTeacher);

            //when
            ValueTask<Teacher> onRemoveTeacherTask = this.teacherService.RemoveTeacherByIdAsync(inputTeacherId);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(onRemoveTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should().BeEquivalentTo(expectedTeacherValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
