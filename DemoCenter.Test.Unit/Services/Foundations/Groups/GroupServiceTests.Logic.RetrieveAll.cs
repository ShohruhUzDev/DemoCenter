using System.Linq;
using DemoCenter.Models.Groups;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllGroups()
        {
            //given
            IQueryable<Group> randomGroups = CreateRandomGroups();
            IQueryable<Group> storageGroups = randomGroups;
            IQueryable<Group> expectedGroups = storageGroups;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllGroups()).Returns(storageGroups);

            //when
            IQueryable<Group> actualGroups =
                this.groupService.RetrieveAllGroups();

            //then
            actualGroups.Should().BeEquivalentTo(expectedGroups);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllGroups(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
