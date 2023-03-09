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
    }
}
