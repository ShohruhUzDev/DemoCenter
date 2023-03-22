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
        public async Task ShouldThrowValidationExceptionOnRetrieveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidStudentId = Guid.Empty;
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.Id),
                values: "Id is required");

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            //when
            ValueTask<Student> onRetrieveStudentTask =
                this.studentService.RetrieveStudentByIdAsync(invalidStudentId);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(onRetrieveStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfStudentNotFoundAndLogItAsync()
        {
            //given
            Guid someStudentId = Guid.NewGuid();
            Student noStudent = null;
            var notFounStudentException = new NotFoundStudentException(someStudentId);

            var expectedStudentValidationException =
                new StudentValidationException(notFounStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noStudent);

            //when
            ValueTask<Student> onRetrieveStudentTask = this.studentService.RetrieveStudentByIdAsync(someStudentId);

            StudentValidationException actualStudentValidationException =
                            await Assert.ThrowsAsync<StudentValidationException>(onRetrieveStudentTask.AsTask);

            //then
            actualStudentValidationException.Should().BeEquivalentTo(expectedStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectStudentByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))), Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
