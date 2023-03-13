using System;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime=GetRandomDateTimeOffset();
            Group randomGroup = CreateRandomGroup(randomDateTime);
            Group invalidGroup = randomGroup;
            var invalidGroupException = new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.UpdatedDate),
                values: $"Date is the same as {nameof(Group.CreatedDate)}");

            var expectedGroupValidationException=
                new GroupValidationException(invalidGroupException);

            this.dateTimeBrokerMock.Setup(broker=>
                broker.GetCurrenDateTime()).Returns(randomDateTime);

            //when
            ValueTask<Group> modifyGroupTask =
                            this.groupService.ModifyGroupAsync(invalidGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrenDateTime(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(
                    expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker=>
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
            DateTimeOffset dateTime = GetRandomDateTimeOffset();
            Group randomGroup = CreateRandomGroup(dateTime);
            Group inputGroup = randomGroup;
            inputGroup.UpdatedDate = dateTime.AddMinutes(minuts);
            var invalidGroupException=new InvalidGroupException();

            invalidGroupException.AddData(
                key: nameof(Group.UpdatedDate),
                values: "Date is not recent.");

            var expectedGroupValidationException=
                new GroupValidationException(invalidGroupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Returns(dateTime);

            //when
            ValueTask<Group> modifyGroupTask=
                this.groupService.ModifyGroupAsync(inputGroup);

            GroupValidationException actualGroupValidationException =
                await Assert.ThrowsAsync<GroupValidationException>(modifyGroupTask.AsTask);

            //then
            actualGroupValidationException.Should()
                .BeEquivalentTo(expectedGroupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptonAs(
                    expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectGroupByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
