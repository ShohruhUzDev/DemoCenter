using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfGroupIsNullAndLogItAsync()
        {
            //given
            Group nullGroup = null;
            var nullGroupException = new NullGroupException();

            var expectedGroupValidationException = new GroupValidationException(nullGroupException);

            //when
            ValueTask<Group> onModifyGroupTask = this.groupService.ModifyGroupAsync(nullGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(onModifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupAsync(It.IsAny<Group>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfGroupIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            var invalidGroup = new Group
            {
                GroupName = invalidString
            };

            var invalidGroupException = new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.Id),
                values: "Id is required");
    
            invalidGroupException.AddData(
               key: nameof(Group.GroupName),
               values: "Text is required");

            invalidGroupException.AddData(
               key: nameof(Group.CreatedDate),
               values: "Value is required");

            invalidGroupException.AddData(
               key: nameof(Group.UpdatedDate),
               values: new[]
               {
                   "Value is required",
                   "Date is not recent.",
                   $"Date is the same as {nameof(Group.CreatedDate)}"});

            var expectedGroupValidationException =
                new GroupValidationException(invalidGroupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime())
                     .Returns(GetRandomDateTimeOffset);


            //when
            ValueTask<Group> modifyGroupTask = this.groupService.ModifyGroupAsync(invalidGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                 broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptonAs(expectedGroupValidationException)))
                    ,Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupAsync(It.IsAny<Group>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
