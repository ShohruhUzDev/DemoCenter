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
        public async Task ShouldThrowValidationExceptionOnRetrieveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidTeacherId = Guid.Empty;
            var invalidTeacherException = new InvalidTeacherException();

            invalidTeacherException.AddData(
                key: nameof(Teacher.Id),
                values: "Id is required");

            var expectedTeacherValidationException =
                new TeacherValidationException(invalidTeacherException);

            //when
            ValueTask<Teacher> onRetrieveTeacherTask =
                this.teacherService.RetrieveTeacherByIdAsync(invalidTeacherId);

            TeacherValidationException actualTeacherValidationException =
                await Assert.ThrowsAsync<TeacherValidationException>(onRetrieveTeacherTask.AsTask);

            //then
            actualTeacherValidationException.Should()
                .BeEquivalentTo(expectedTeacherValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
