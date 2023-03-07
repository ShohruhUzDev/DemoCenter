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
    }
}