using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        [Fact]
        public async Task ShouldModifyGroupAsync()
        {
            //given
            DateTimeOffset randomDate=GetRandomDateTimeOffset();
            Group randomGroup=CreateRandomModifyGroup(randomDate);
            Group inputGroup = randomGroup;
            Group storageGroup = inputGroup.DeepClone();
            storageGroup.UpdatedDate = randomGroup.CreatedDate;
            Group updatedGroup = inputGroup;
            Group expectedGroup = updatedGroup.DeepClone();
            Guid groupId=inputGroup.Id;

            this.dateTimeBrokerMock.Setup(broker=>
            broker.GetCurrenDateTime()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
            broker.SelectGroupByIdAsync(groupId)).ReturnsAsync(storageGroup);

            this.storageBrokerMock.Setup(broker=>
            broker.UpdateGroupAsync(inputGroup)).ReturnsAsync(updatedGroup);

            //when
            Group actualGroup = await
                this.groupService.ModifyGroupAsync(inputGroup);

            //then
            actualGroup.Should().BeEquivalentTo(expectedGroup);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrenDateTime(), Times.Never);    

            this.storageBrokerMock.Verify(broker=>
            broker.SelectGroupByIdAsync(groupId), Times.Once());    

            this.storageBrokerMock.Verify(broker=>
            broker.UpdateGroupAsync(inputGroup), Times.Once()); 

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
