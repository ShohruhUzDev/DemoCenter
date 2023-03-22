using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDepedencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Group someGroup = CreateRandomGroup();
            SqlException sqlException = CreateSqlException();
            var failedGroupStorageException = new FailedGroupStorageException(sqlException);

            var expectedDependencyException =
                new GroupDependencyException(failedGroupStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Throws(sqlException);

          
            //when
            ValueTask<Group> addGroupTask=this.groupService.AddGroupAsync(someGroup);

            GroupDependencyException actualGroupDependencyException =
                await Assert.ThrowsAsync<GroupDependencyException>(addGroupTask.AsTask);

            //then
            actualGroupDependencyException.Should().BeEquivalentTo(expectedDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptonAs(
                    expectedDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
