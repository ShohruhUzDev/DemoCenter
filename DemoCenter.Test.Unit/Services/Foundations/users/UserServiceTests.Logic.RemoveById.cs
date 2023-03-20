using System;
using System.Threading.Tasks;
using DemoCenter.Models.Users;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldRemoveUserByIdAsync()
        {
            //given
            Guid randomId = Guid.NewGuid();
            Guid inputUserId = randomId;
            User randomUser = CreateRandomUser();
            User storageUser = randomUser;
            User expectedInputUser = storageUser;
            User deletedUser = expectedInputUser;
            User expectedUser = deletedUser.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(storageUser);
            
            this.storageBrokerMock.Setup(broker=>
                broker.DeleteUserAsync(expectedInputUser))
                    .ReturnsAsync(deletedUser);

            //when
            User actualUser=await this.userService.RemoveUserByIdAsync(inputUserId);

            //then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectUserByIdAsync(inputUserId), Times.Once()); 

            this.storageBrokerMock.Verify(broker=>
                broker.DeleteUserAsync(expectedInputUser), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
