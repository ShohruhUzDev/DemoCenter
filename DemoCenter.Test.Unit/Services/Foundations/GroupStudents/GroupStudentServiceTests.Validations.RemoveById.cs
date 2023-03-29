using DemoCenter.Models.Groups.Exceptions;
using Moq;
using System;
using System.Threading.Tasks;
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
            Guid invalidPostId = Guid.Empty;

            var invalidGroupStudentException = new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.GroupId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.PostId),
                values: "Id is required");

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            // when
            ValueTask<GroupStudent> removeGroupStudentByIdTask =
                this.GroupStudentService.RemoveGroupStudentByIdAsync(invalidGroupId, invalidPostId);

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
        }


    }
}
