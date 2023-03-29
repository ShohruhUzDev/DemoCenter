using Moq;
using System.Threading.Tasks;
using System;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidPostId = Guid.Empty;
            Guid invalidProfileId = Guid.Empty;

            var invalidGroupStudentException = new InvalidGroupStudentException();

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.PostId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.ProfileId),
                values: "Id is required");

            var expectedGroupStudentValidationException = new
                GroupStudentValidationException(invalidGroupStudentException);

            // when
            ValueTask<GroupStudent> retrieveGroupStudentByIdTask =
                this.GroupStudentService.RetrieveGroupStudentByIdAsync(invalidPostId, invalidProfileId);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    retrieveGroupStudentByIdTask.AsTask);

            // then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(invalidPostId, invalidProfileId),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }


    }
}
