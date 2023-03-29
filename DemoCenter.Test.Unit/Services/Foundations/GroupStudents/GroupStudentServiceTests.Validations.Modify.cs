using DemoCenter.Models.Groups.Exceptions;
using DemoCenter.Models.GroupStudents;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
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
                PostId = invalidId,
                ProfileId = invalidId
            };

            var invalidGroupStudentException = new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.PostId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.ProfileId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.CreatedDate),
                values: "Date is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.UpdatedDate),
                 values: new[]
                    {
                        "Date is required",
                        $"Date is the same as {nameof(GroupStudent.CreatedDate)}"
                    });

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(invalidGroupStudentException);

            //when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.GroupStudentService.ModifyGroupStudentAsync(invalidGroupStudent);

            GroupStudentValidationException actualPostValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    modifyGroupStudentTask.AsTask);

            //then
            actualPostValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupStudentAsync(invalidGroupStudent), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
