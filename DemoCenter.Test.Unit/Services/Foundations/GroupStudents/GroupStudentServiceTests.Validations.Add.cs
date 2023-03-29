using System;
using System.Threading.Tasks;
using DemoCenter.Models.Groups.Exceptions;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Models.GroupStudents.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfGroupStudentIsNullAndLogItAsync()
        {
            // given
            GroupStudent nullGroupStudent = null;

            var nullGroupStudentException =
                new NullGroupStudentException();

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(nullGroupStudentException);

            // when
            ValueTask<GroupStudent> addGrouPostTask =
                this.groupStudentService.AddGroupStudentAsync(nullGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    addGrouPostTask.AsTask);

            // then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfGroupStudentIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidGuid = Guid.Empty;

            var invalidGroupStudent = new GroupStudent
            {
                GroupId = invalidGuid,
                StudentId = invalidGuid
            };

            var invalidGroupStudentException =
                new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.GroupId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.StudentId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.CreatedDate),
                values: "Value is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.UpdatedDate),
                values: "Value is required");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            //when
            ValueTask<GroupStudent> addGroupStudentTask =
                this.groupStudentService.AddGroupStudentAsync(invalidGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    addGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(invalidGroupStudent),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            //given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(randomDateTimeOffset);
            GroupStudent invalidGroupStudent = randomGroupStudent;

            invalidGroupStudent.UpdatedDate =
                invalidGroupStudent.CreatedDate.AddDays(randomNumber);

            var invalidGroupStudentException =
                new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.UpdatedDate),
                values: $"Date is not the same as {nameof(GroupStudent.CreatedDate)}");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Returns(randomDateTimeOffset);

            //when
            ValueTask<GroupStudent> addGroupStudentTask =
                this.groupStudentService.AddGroupStudentAsync(invalidGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    addGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }


        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTime();

            DateTimeOffset invalidDateTime =
                randomDateTime.AddMinutes(invalidSeconds);

            GroupStudent randomGroupStudent = CreateRandomGroupStudent(invalidDateTime);
            GroupStudent invalidGroupStudent = randomGroupStudent;

            var invalidGroupStudentException =
                new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.CreatedDate),
                values: "Date is not recent");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Returns(randomDateTime);

            // when
            ValueTask<GroupStudent> addGroupStudentTask =
                this.groupStudentService.AddGroupStudentAsync(invalidGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
               await Assert.ThrowsAsync<GroupStudentValidationException>(
                   addGroupStudentTask.AsTask);

            // then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
