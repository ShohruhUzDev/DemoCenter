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
            
            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedTeacherValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
