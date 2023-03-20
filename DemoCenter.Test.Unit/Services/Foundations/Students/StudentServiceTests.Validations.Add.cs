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

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStudentAsync(It.IsAny<Student>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            var invalidStduent = new Student
            {
                FirstName = invalidString,
                LastName = invalidString,
                Phone = invalidString,
            };
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.Id),
                values: "Id is required");

            invalidStudentException.AddData(
                key: nameof(Student.FirstName),
                values: "Text is required");

            invalidStudentException.AddData(
                key: nameof(Student.LastName),
                values: "Text is required");

            invalidStudentException.AddData(
                key: nameof(Student.Phone),
                values: "Text is required");

            invalidStudentException.AddData(
                key: nameof(Student.CreatedDate),
                values: "Value is required");

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: "Value is required");

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            //when
            ValueTask<Student> addStudentTask = this.studentService.AddStudentAsync(invalidStduent);

            StudentValidationException actualStudentValidatioException =
                await Assert.ThrowsAsync<StudentValidationException>(addStudentTask.AsTask);

            //then
            actualStudentValidatioException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStudentAsync(It.IsAny<Student>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameUpdatedDateAndLogItAsync()
        {
            //given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Student randomStudent = CreateRandomStudent(randomDate);
            Student invalidStudent = randomStudent;
            invalidStudent.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.CreatedDate),
                values: $"Date is not same as {nameof(Student.UpdatedDate)}");

            var expectedStudentValidationException = new StudentValidationException(invalidStudentException);

            //when
            ValueTask<Student> addStudentTask = this.studentService.AddStudentAsync(invalidStudent);

            var actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(addStudentTask.AsTask);

            //then
            actualStudentValidationException.Should().BeEquivalentTo(expectedStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStudentAsync(It.IsAny<Student>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
