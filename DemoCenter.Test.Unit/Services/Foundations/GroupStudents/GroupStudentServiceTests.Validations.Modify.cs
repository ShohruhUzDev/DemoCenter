using Moq;
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
                this.GroupStudentService.ModifyGroupStudentAsync(nullGroupStudent);

            GroupStudentValidationException actualGroupStudentValidationException =
                await Assert.ThrowsAsync<GroupStudentValidationException>(
                    modifyGroupStudentTask.AsTask);

            //then
            actualGroupStudentValidationException.Should().BeEquivalentTo(
                expectedPostValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedPostValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupStudentAsync(nullGroupStudent), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
