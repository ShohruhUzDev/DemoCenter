using DemoCenter.Models.GroupStudents;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Taarafo.Core.Models.GroupStudents.Exceptions;
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
    }
}
