using Moq;
using System.Threading.Tasks;
using System;
using Xunit;
using DemoCenter.Models.Groups.Exceptions;
using DemoCenter.Models.GroupStudents;
using FluentAssertions;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
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

            var expectedGroupStudentValidationException = new
                GroupStudentValidationException(invalidGroupStudentException);

            // when
            ValueTask<GroupStudent> retrieveGroupStudentByIdTask =
                this.groupStudentService.RetrieveGroupStudentByIdAsync(invalidGroupId, invalidStudentId);

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
                broker.SelectGroupStudentByIdAsync(invalidGroupId, invalidStudentId),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }


    }
}
