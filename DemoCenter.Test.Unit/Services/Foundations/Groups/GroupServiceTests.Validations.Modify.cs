using System;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
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
                broker.LogError(It.Is(SameExceptionAs(expectedGroupValidationException))), Times.Once);

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
                   "Date is not recent",
                   $"Date is the same as {nameof(Group.CreatedDate)}"});

            var expectedGroupValidationException =
                new GroupValidationException(invalidGroupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                     .Returns(GetRandomDateTime);


            //when
            ValueTask<Group> modifyGroupTask = this.groupService.ModifyGroupAsync(invalidGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                 broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedGroupValidationException)))
                    , Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupAsync(It.IsAny<Group>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Group randomGroup = CreateRandomGroup(randomDateTime);
            Group invalidGroup = randomGroup;
            var invalidGroupException = new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.UpdatedDate),
                values: $"Date is the same as {nameof(Group.CreatedDate)}");

            var expectedGroupValidationException =
                new GroupValidationException(invalidGroupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Group> modifyGroupTask =
                            this.groupService.ModifyGroupAsync(invalidGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(invalidGroup.Id), Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutsBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdateDateIsNotRecentAndLogItAsync(int minuts)
        {
            //given
            DateTimeOffset dateTime = GetRandomDateTime();
            Group randomGroup = CreateRandomGroup(dateTime);
            Group inputGroup = randomGroup;
            inputGroup.UpdatedDate = dateTime.AddMinutes(minuts);
            var invalidGroupException = new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.UpdatedDate),
                values: "Date is not recent");

            var expectedGroupValidationException =
                new GroupValidationException(invalidGroupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(dateTime);

            //when
            ValueTask<Group> modifyGroupTask =
                this.groupService.ModifyGroupAsync(inputGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfGroupDoesNotExistAndLogItAsync()
        {
            //given
            int randomNegativeNumber = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            Group randomGroup = CreateRandomGroup(dateTime);
            Group nonExistGroup = randomGroup;
            nonExistGroup.CreatedDate = dateTime.AddMinutes(randomNegativeNumber);
            Group nullGroup = null;

            var notFoundGroupException = new NotFoundGroupException(nonExistGroup.Id);

            var expectedGroupValidationException =
                new GroupValidationException(notFoundGroupException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(nonExistGroup.Id)).ReturnsAsync(nullGroup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(dateTime);

            //when
            ValueTask<Group> modifyGroupTask =
                this.groupService.ModifyGroupAsync(nonExistGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(nonExistGroup.Id), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            //given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Group randomGroup = CreateRandomModifyGroup(randomDateTime);
            Group invalidGroup = randomGroup.DeepClone();
            Group storageGroup = invalidGroup.DeepClone();
            storageGroup.CreatedDate = storageGroup.CreatedDate.AddMinutes(randomMinutes);
            storageGroup.UpdatedDate = storageGroup.UpdatedDate.AddMinutes(randomMinutes);
            var invalidGroupException = new InvalidGroupException();
            Guid groupId = invalidGroup.Id;

            invalidGroupException.AddData(
                key: nameof(Group.CreatedDate),
                values: $"Date is not same as {nameof(Group.CreatedDate)}");

            var expectedGroupValidationException =
                new GroupValidationException(invalidGroupException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(groupId)).ReturnsAsync(storageGroup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Group> modifyGroup = this.groupService.ModifyGroupAsync(invalidGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroup.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(groupId), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdateDateSameAsUpdateDateAndLogItAsync()
        {
            //given
            DateTimeOffset radomDateTime = GetRandomDateTime();
            Group randomGroup = CreateRandomModifyGroup(radomDateTime);
            Group invalidGroup = randomGroup;
            Group storageGroup = randomGroup.DeepClone();
            invalidGroup.UpdatedDate = storageGroup.UpdatedDate;
            Guid groupId = invalidGroup.Id;
            var invalidGroupException = new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.UpdatedDate),
                values: $"Date is the same as {nameof(Group.UpdatedDate)}");

            var expectedGroupValidationException =
                new GroupValidationException(invalidGroupException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupByIdAsync(invalidGroup.Id)).ReturnsAsync(storageGroup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(radomDateTime);

            //when
            ValueTask<Group> modifyGroupTask = this.groupService.ModifyGroupAsync(invalidGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupByIdAsync(groupId), Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupValidationException))), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
