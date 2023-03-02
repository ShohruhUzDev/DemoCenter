using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Services.Foundations
{
    public partial class GroupServiceTest
    {
        [Fact]
        public async Task ShouldAddGroupAsync()
        {
            //given
            Group randomGroup = CreateRandomGroup();
            Group inputGroup = randomGroup;
            Group persistedGroup = inputGroup;
            Group expectedGroup = persistedGroup.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.InsertGroupAsync(inputGroup)).ReturnsAsync(persistedGroup);

            //when
            Group actualGroup = await this.groupService
                .AddGroupAsync(inputGroup);

            //then
            actualGroup.Should().BeEquivalentTo(expectedGroup);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertGroupAsync(inputGroup), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
