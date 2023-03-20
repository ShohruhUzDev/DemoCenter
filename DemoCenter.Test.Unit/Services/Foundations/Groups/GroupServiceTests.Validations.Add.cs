using System;
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
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            //given
            Group nullGroup = null;
            var nullGroupException = new NullGroupException();
            var expectedGroupValidationException =
                new GroupValidationException(nullGroupException);

            //when
            ValueTask<Group> addGroupTask = this.groupService.AddGroupAsync(nullGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(addGroupTask.AsTask);

            //then
            actualGroupValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupAsync(It.IsAny<Group>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfInputGroupIsInvalidAndLogItAsync(
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
                values: "Value is required");

            var expectedGroupValidationException =
                new GroupValidationException(invalidGroupException);

            //when
            ValueTask<Group> addGroupTask = this.groupService.AddGroupAsync(invalidGroup);

            GroupValidationException actualValidationException =
               await Assert.ThrowsAsync<GroupValidationException>(addGroupTask.AsTask);

            //then
            actualValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(
                    expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupAsync(It.IsAny<Group>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            //given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Group randoGroup = CreateRandomGroup(randomDate);
            Group invalidGroup = randoGroup;
            invalidGroup.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidGroupException = new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.CreatedDate),
                values: $"Date is not same as {nameof(Group.UpdatedDate)}");

            var expectedGroupValidationException = new GroupValidationException(invalidGroupException);

            //when
            ValueTask<Group> addGroupTask = this.groupService.AddGroupAsync(invalidGroup);

            var actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(addGroupTask.AsTask);

            //then
            actualGroupValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(expectedGroupValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupAsync(It.IsAny<Group>()), Times.Never());

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}