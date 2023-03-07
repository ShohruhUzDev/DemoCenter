using System.Threading.Tasks;
using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAnnLogItAsync()
        {
            //given
            Student nullStudent = null;
            var nullStudentException = new NullStudentException();
            var expectedStudentValidationException = new
                StudentValidationException(nullStudentException);

            //when
            ValueTask<Student> addStudentTask =
                this.studentService.AddStudentAsync(nullStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(addStudentTask.AsTask);

            //then
            actualStudentValidationException.Should().
                BeEquivalentTo(expectedStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker=>
            broker.InsertStudentAsync(It.IsAny<Student>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();

        }
    }
}
