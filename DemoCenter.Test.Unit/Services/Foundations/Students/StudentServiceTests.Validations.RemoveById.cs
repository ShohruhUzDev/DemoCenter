using System;
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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidStudentId = Guid.Empty;
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.Id),
                values: "Id is required");

            var expectedGroupValidationException = new StudentValidationException(invalidStudentException);

            //when
            ValueTask<Student> onRemoveStudentTask = this.studentService.RemoveStudentByIdAsync(invalidStudentId);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(onRemoveStudentTask.AsTask);

            //then
            actualStudentValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteStudentAsync(It.IsAny<Student>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfStudentIsNotFoundAndLogItAsync()
        {
            //given
            Guid randomStudentId = Guid.NewGuid();
            Guid inputStudentId = randomStudentId;
            Student noStudent = null;
            var notFoundStudentException = new NotFoundStudentException(inputStudentId);

            var expectedStudentValidationException =
                new StudentValidationException(notFoundStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noStudent);

            //when
            ValueTask<Student> onRemoveStudentTask = this.studentService.RemoveStudentByIdAsync(inputStudentId);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(onRemoveStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                 .BeEquivalentTo(expectedStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
