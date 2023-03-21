using System;
using System.Threading.Tasks;
using DemoCenter.Models.GroupStudents;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentTests
    {
        [Fact]
        public async Task ShouldRetrieveGroupStudentByIdAsync()
        {
            //given
            Guid randomGroupId= Guid.NewGuid();
            Guid inputGroupId = randomGroupId;
            Guid randomStudentId = Guid.NewGuid();
            Guid inputStudentId = randomStudentId;
            GroupStudent randomGroupStudent = CreateRandomGroupStudent();
            GroupStudent storedGroupStudent = randomGroupStudent;
            GroupStudent expectedGroupStudent = storedGroupStudent.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(inputGroupId, inputStudentId))
                    .ReturnsAsync(storedGroupStudent);

            //when
            GroupStudent actualGroupStudent = await
                this.groupStudentService.RetrieveGroupStudentByIdAsync(inputGroupId, inputStudentId);
            
            //then
            actualGroupStudent.Should().BeEquivalentTo(expectedGroupStudent);

            this.storageBrokerMock.Verify(broker=>
                broker.SelectGroupStudentByIdAsync(inputGroupId, inputStudentId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
