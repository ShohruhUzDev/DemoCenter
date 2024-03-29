﻿using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Students;
using DemoCenter.Models.Foundations.Students.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStudentIsNullAndLogItAsync()
        {
            //given
            Student nullStudent = null;
            var nullStudentException = new NullStudentException();

            var expectedStudentValidationException = new StudentValidationException(nullStudentException);

            //when
            ValueTask<Student> onModifyStudentTask = this.studentService.ModifyStudentAsync(nullStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(onModifyStudentTask.AsTask);

            //then
            actualStudentValidationException.Should().BeEquivalentTo(expectedStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateStudentAsync(It.IsAny<Student>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfStudentIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            var invalidStudent = new Student
            {
                FirstName = invalidString,
                LastName = invalidString,
                Phone = invalidString
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
                values: new[]
                {
                    "Value is required",
                    "Date is not recent",
                    $"Date is the same as {nameof( Student.CreatedDate)}"
                });

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(GetRandomDateTime);

            //when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    modifyStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateStudentAsync(It.IsAny<Student>()), Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Student randomStudent = CreateRandomStudent(randomDateTime);
            Student invalidStudent = randomStudent;
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: $"Date is the same as {nameof(Student.CreatedDate)}");

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(modifyStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                  .BeEquivalentTo(expectedStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(invalidStudent.Id), Times.Never());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShoulThrowValidationExceptinOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Student randomStudent = CreateRandomStudent(randomDateTime);
            Student inputStudent = randomStudent;
            inputStudent.UpdatedDate = randomDateTime.AddMinutes(invalidSeconds);
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: "Date is not recent");

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(inputStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(modifyStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>()), Times.Never());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();

        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnMofifyIsStudentDoesNotFoundAndLogItAsync()
        {
            //given
            int randomNegativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Student randomStudent = CreateRandomStudent(randomDateTime);
            Student nonExistStudent = randomStudent;
            nonExistStudent.CreatedDate = randomDateTime.AddMinutes(randomNegativeMinutes);
            Student nullStudent = null;
            var notFoundStudentException = new NotFoundStudentException(nonExistStudent.Id);

            var expectedStudentValidationException =
                new StudentValidationException(notFoundStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(nonExistStudent.Id)).ReturnsAsync(nullStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Student> modifyStudentTask = this.studentService.ModifyStudentAsync(nonExistStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(modifyStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(nonExistStudent.Id), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());
            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            //given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Student randomStudent = CreateRandomModifyStudent(randomDateTime);
            Student invalidStudent = randomStudent.DeepClone();
            Student storageStudent = invalidStudent.DeepClone();
            storageStudent.CreatedDate = storageStudent.CreatedDate.AddMinutes(randomMinutes);
            storageStudent.UpdatedDate = storageStudent.UpdatedDate.AddMinutes(randomMinutes);
            var invalidStudentException = new InvalidStudentException();
            Guid studentId = invalidStudent.Id;

            invalidStudentException.AddData(
                key: nameof(Student.CreatedDate),
                values: $"Date is not same as {nameof(Student.CreatedDate)}");

            var expectedStudentValidatinException =
                new StudentValidationException(invalidStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(studentId)).ReturnsAsync(storageStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(modifyStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidatinException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(studentId), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());
            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidatinException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Student randomStudent = CreateRandomModifyStudent(randomDateTime);
            Student invalidStudent = randomStudent;
            Student storageStudent = randomStudent.DeepClone();
            invalidStudent.UpdatedDate = storageStudent.UpdatedDate;
            Guid studentId = invalidStudent.Id;
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: $"Date is the same as {nameof(Student.UpdatedDate)}");

            var expectedStudenValidationException =
                new StudentValidationException(invalidStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(invalidStudent.Id)).ReturnsAsync(storageStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(modifyStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudenValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(studentId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudenValidationException))), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
