using System;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveByIdGroupAsync()
        {
            //given
            Guid randomGroupId = Guid.NewGuid();
            Guid inputGroupId = randomGroupId;
            Group randomGroup = CreateRandomGroup();
            Group storedGroup = randomGroup;
            Group expectedGroup = storedGroup.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(inputGroupId)).ReturnsAsync(storedGroup);

            //when
            Group actualGroup = await this.groupService.RetrieveGroupByIdAsync(inputGroupId);

            //then
            actualGroup.Should().BeEquivalentTo(expectedGroup);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(inputGroupId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
