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
        public async Task ShouldModifyUserAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            User randomUser = CreateRandomModifyUser(randomDateTime);
            User inputUser = randomUser;
            User storageUser = inputUser.DeepClone();
            storageUser.UpdatedDate = randomUser.CreatedDate;
            User updatedUser = inputUser;
            User expectedUser = updatedUser.DeepClone();
            Guid userId = inputUser.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime())
                    .Returns(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(userId))
                    .ReturnsAsync(storageUser);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateUserAsync(inputUser))
                    .ReturnsAsync(updatedUser);

            //when
            User actualUser = 
                await this.userService.ModifyUserAsync(inputUser);

            //then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(userId),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(inputUser),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
