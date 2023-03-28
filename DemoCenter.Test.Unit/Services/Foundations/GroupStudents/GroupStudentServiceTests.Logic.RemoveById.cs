using System;
using System.Threading.Tasks;
using DemoCenter.Models.GroupStudents;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldRemoveGroupStudentByIdAsync()
        {
            //given
            Guid randomStudentId = Guid.NewGuid();
            Guid randomGroupId = Guid.NewGuid();
            Guid inputStudentId = randomStudentId;
            Guid inputGroupId = randomGroupId;
            GroupStudent randomGroupStudent = CreateRandomGroupStudent();
            GroupStudent storageGroupStudent = randomGroupStudent;
            GroupStudent expectedInputGroupStudent = storageGroupStudent;
            GroupStudent deletedGroupStudent = expectedInputGroupStudent;
            GroupStudent expectedGroupStudent = deletedGroupStudent.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(inputGroupId, inputStudentId))
                    .ReturnsAsync(storageGroupStudent);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteGroupStudentAsync(expectedInputGroupStudent))
                    .ReturnsAsync(deletedGroupStudent);

            //when
            GroupStudent actualGroupStudent = await
                this.groupStudentService.RemoveGroupStudentByIdAsync(inputGroupId, inputStudentId);

            //then
            actualGroupStudent.Should().BeEquivalentTo(expectedGroupStudent);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(inputGroupId, inputStudentId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGroupStudentAsync(expectedInputGroupStudent), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
