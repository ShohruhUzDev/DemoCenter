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
        public async Task ShouldRemoveGroupByIdAsync()
        {
            //given
            Guid randomGroupId = Guid.NewGuid();
            Guid inputGroupId = randomGroupId;
            Group randomGroup = CreateRandomGroup();
            Group storageGroup = randomGroup;
            Group expectedInputGroup = storageGroup;
            Group deletedGroup = expectedInputGroup;
            Group expectedGroup = deletedGroup.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.SelectGroupByIdAsync(inputGroupId)).ReturnsAsync(storageGroup);

            this.storageBrokerMock.Setup(broker =>
            broker.DeleteGroupAsync(expectedInputGroup)).ReturnsAsync(deletedGroup);

            //when
            Group actualGroup = await this.groupService.RemoveGroupByIdAsync(inputGroupId);

            //then
            actualGroup.Should().BeEquivalentTo(expectedGroup);

            this.storageBrokerMock.Verify(broker =>
            broker.SelectGroupByIdAsync(inputGroupId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
            broker.DeleteGroupAsync(expectedInputGroup), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
