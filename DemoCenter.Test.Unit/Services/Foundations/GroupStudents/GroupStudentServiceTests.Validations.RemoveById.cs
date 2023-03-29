using System;
using System.Threading.Tasks;
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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidGroupId = Guid.Empty;
            Guid invalidStudentId = Guid.Empty;

            var invalidGroupStudentException = new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.GroupId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.StudentId),
                values: "Id is required");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            // when
            ValueTask<GroupStudent> removeGroupStudentByIdTask =
                this.groupStudentService.RemoveGroupStudentByIdAsync(invalidGroupId, invalidStudentId);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    removeGroupStudentByIdTask.AsTask);

            // then
            actualGroupStudentValidationException.Should()
                .BeEquivalentTo(expectedGroupStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIdsGroupStudentIsNotFoundAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(randomDateTime);
            Guid inputGroupId = randomGroupStudent.GroupId;
            Guid inputStudentId = randomGroupStudent.StudentId;
            GroupStudent nullStorageGroupStudent = null;

            var notFoundGroupStudentException =
                new NotFoundGroupStudentException(inputGroupId, inputStudentId);

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(notFoundGroupStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(inputGroupId, inputStudentId))
                    .ReturnsAsync(nullStorageGroupStudent);

            //when
            ValueTask<GroupStudent> removeGroupStudentTask =
                this.groupStudentService.RemoveGroupStudentByIdAsync(inputGroupId, inputStudentId);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    removeGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGroupStudentAsync(It.IsAny<GroupStudent>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
