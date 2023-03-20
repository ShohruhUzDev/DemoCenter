using System;
using System.Threading.Tasks;
using DemoCenter.Models.Users;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveByIdUserAsync()
        {
            //given
            Guid randomUserId = Guid.NewGuid();
            Guid inputUserId = randomUserId;
            User randomUser = CreateRandomUser();
            User storedUser = randomUser;
            User expectedUser = storedUser.DeepClone();

            this.storageBrokerMock.Setup(broker=>
                broker.SelectUserByIdAsync(inputUserId)).ReturnsAsync(storedUser);

            //when
            User actualUser=await this.userService.RetrieveUserByIdAsync(inputUserId);

            //then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectUserByIdAsync(inputUserId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
