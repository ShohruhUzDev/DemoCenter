using DemoCenter.Models.GroupStudents;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using DemoCenter.Models.Groups.Exceptions;
using Xunit;
using System;

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
                key: nameof(GroupStudent.Group),
                values: "Object is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.StudentId),
                values: "Id is required");

            invalidGroupStudentException.AddData(
                key: nameof(GroupStudent.Student),
                values: "Object is required");

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
                    Times.Never);

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
    }
}
