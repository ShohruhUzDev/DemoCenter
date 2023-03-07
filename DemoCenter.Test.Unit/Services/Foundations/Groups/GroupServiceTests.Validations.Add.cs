using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions;
using Microsoft.Identity.Client;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            //given
            Group nullGroup = null;
            var nullGroupException = new NullGroupException();
            var expectedGroupValidationException =
                new GroupValidationException(nullGroupException);

            //when
            ValueTask<Group> addGroupTask = this.groupService.AddGroupAsync(nullGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(addGroupTask.AsTask);

            //then
            actualGroupValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptonAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertGroupAsync(It.IsAny<Group>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
