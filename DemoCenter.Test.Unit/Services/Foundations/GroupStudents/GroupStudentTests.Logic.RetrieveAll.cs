using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.GroupStudents;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentTests
    {
        [Fact]
        public async Task ShouldRetrieveAllGroupStudentsAsync()
        {
            //given
            IQueryable<GroupStudent> randomGroupStudents = CreateRandomGroupStudents();
            IQueryable<GroupStudent> storageGroupStudents = randomGroupStudents;
            IQueryable<GroupStudent> expectedGroupStudents = storageGroupStudents;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllGroupStudents())
                    .Returns(storageGroupStudents);

            //when
            IQueryable<GroupStudent> actualGroupStudents =
                this.groupStudentService.RetrieveAllGroupStudents();

            //then
            actualGroupStudents.Should().BeEquivalentTo(expectedGroupStudents);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllGroupStudents(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
