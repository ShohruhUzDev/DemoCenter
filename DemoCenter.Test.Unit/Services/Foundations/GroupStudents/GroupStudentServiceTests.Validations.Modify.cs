using System;
using System.Threading.Tasks;
using DemoCenter.Models.Groups.Exceptions;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Models.GroupStudents.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfGroupStudentIsNullAndLogItAsync()
        {
            //given
            GroupStudent nullGroupStudent = null;
            var nullGroupStudentException = new NullGroupStudentException();

            GroupStudentValidationException expectedPostValidationException =
                new GroupStudentValidationException(nullGroupStudentException);

            //when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.groupStudentService.ModifyGroupStudentAsync(nullGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    modifyGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedPostValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupStudentAsync(nullGroupStudent), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        public async Task ShouldThrowValidationExceptionOnModifyIfGroupStudentIsInvalidAndLogItAsync(
            Guid invalidId)
        {
            //given
            var invalidGroupStudent = new GroupStudent
            {
                StudentId = invalidId,
                GroupId = invalidId
            };

            var invalidGroupStudentException = new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.StudentId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.GroupId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.CreatedDate),
                values: "Value is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.UpdatedDate),
                 values: new[]
                    {
                        "Value is required",
                        $"Date is the same as {nameof(GroupStudent.CreatedDate)}"
                    });

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            //when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.groupStudentService.ModifyGroupStudentAsync(invalidGroupStudent);

            GroupStudentValidationException actualPostValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    modifyGroupStudentTask.AsTask);

            //then
            actualPostValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupStudentAsync(invalidGroupStudent), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(randomDateTime);
            GroupStudent invalidGroupStudent = randomGroupStudent;
            var invalidGroupStudentException = new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.UpdatedDate),
                values: $"Date is the same as {nameof(GroupStudent.CreatedDate)}");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.groupStudentService.ModifyGroupStudentAsync(invalidGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(modifyGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(
                    invalidGroupStudent.GroupId, invalidGroupStudent.StudentId), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minuts)
        {
            //given
            DateTimeOffset dateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(dateTime);
            GroupStudent inputGroupStudent = randomGroupStudent;
            inputGroupStudent.UpdatedDate = dateTime.AddMinutes(minuts);
            var invalidGroupStudentException = new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.UpdatedDate),
                values: "Date is not recent");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(dateTime);

            //when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.groupStudentService.ModifyGroupStudentAsync(inputGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    modifyGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfGroupStudentDoesNotExistAndLogItAsync()
        {
            //given
            int randomNegativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(dateTime);
            GroupStudent nonExistGroupStudent = randomGroupStudent;
            nonExistGroupStudent.CreatedDate = dateTime.AddMinutes(randomNegativeMinutes);
            GroupStudent nullGroupStudent = null;

            var notFoundGroupStudentException =
                new NotFoundGroupStudentException(
                    nonExistGroupStudent.GroupId,
                    nonExistGroupStudent.StudentId);

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(notFoundGroupStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(nonExistGroupStudent.GroupId,
                    nonExistGroupStudent.StudentId)).ReturnsAsync(nullGroupStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(dateTime);

            //when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.groupStudentService.ModifyGroupStudentAsync(nonExistGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(modifyGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(
                    nonExistGroupStudent.GroupId,
                    nonExistGroupStudent.StudentId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            //given
            int randomNumber = GetRandomNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomModifyGroupStudent(randomDateTime);
            GroupStudent invalidGroupStudent = randomGroupStudent.DeepClone();
            GroupStudent storageGroupStudent = randomGroupStudent.DeepClone();
            storageGroupStudent.CreatedDate = storageGroupStudent.CreatedDate.AddMinutes(randomMinutes);
            storageGroupStudent.UpdatedDate = storageGroupStudent.UpdatedDate.AddMinutes(randomMinutes);
            Guid groupId = invalidGroupStudent.GroupId;
            Guid studentId = invalidGroupStudent.StudentId;

            var invalidGroupStudentException =
                new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.CreatedDate),
                values: $"Date is not the same as {nameof(GroupStudent.CreatedDate)}");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(groupId, studentId))
                    .ReturnsAsync(storageGroupStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.groupStudentService.ModifyGroupStudentAsync(invalidGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(modifyGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(invalidGroupStudent.GroupId,
                    invalidGroupStudent.StudentId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedGroupStudentValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomModifyGroupStudent(randomDateTime);
            GroupStudent invalidGroupStudent = randomGroupStudent;
            GroupStudent storageGroupStudent = invalidGroupStudent.DeepClone();
            invalidGroupStudent.UpdatedDate = storageGroupStudent.UpdatedDate;
            var invalidGroupStudentException = new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.UpdatedDate),
                values: $"Date is the same as {nameof(GroupStudent.UpdatedDate)}");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(
                    invalidGroupStudent.GroupId, invalidGroupStudent.StudentId)).ReturnsAsync(storageGroupStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.groupStudentService.ModifyGroupStudentAsync(invalidGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(modifyGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(
                    invalidGroupStudent.GroupId, invalidGroupStudent.StudentId), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
