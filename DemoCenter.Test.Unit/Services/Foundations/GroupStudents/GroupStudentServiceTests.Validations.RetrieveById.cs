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

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfTeamIsNotFoundAndLogItAsync()
        {
            //given
            Guid someGroupId = Guid.NewGuid();
            Guid someStudentId = Guid.NewGuid();
            GroupStudent noGroupStudent = null;

            var notFoundGroupStudentException =
                new NotFoundGroupStudentException(someGroupId, someStudentId);

            var expectedGroupStudentValidationException =
                new GroupStudentValidationException(notFoundGroupStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(someGroupId, someStudentId))
                    .ReturnsAsync(noGroupStudent);

            //when
            ValueTask<GroupStudent> retrieveGroupStudentByIdTask =
                this.groupStudentService.RetrieveGroupStudentByIdAsync(someGroupId, someStudentId);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    retrieveGroupStudentByIdTask.AsTask);

            // then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedGroupStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(someGroupId, someStudentId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
