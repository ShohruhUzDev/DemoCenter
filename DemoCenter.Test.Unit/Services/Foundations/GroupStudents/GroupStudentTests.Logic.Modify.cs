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
        public async Task ShouldModifyGroupStudentAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent=CreateRandomGroupStudent();
            GroupStudent inputGroupStudent = randomGroupStudent;
            GroupStudent storageGroupStudent = inputGroupStudent.DeepClone();
            storageGroupStudent.UpdatedDate = randomGroupStudent.CreatedDate;
            GroupStudent updatedGroupStudent = inputGroupStudent;
            GroupStudent expectedGroupStudent = updatedGroupStudent.DeepClone();
            Guid GroupId = inputGroupStudent.GroupId;
            Guid StudentId = inputGroupStudent.StudentId;


            this.storageBrokerMock.Setup(broker=>
                broker.SelectGroupStudentByIdAsync(GroupId, StudentId))
                    .ReturnsAsync(storageGroupStudent);

            this.storageBrokerMock.Setup(broker=>
                broker.UpdateGroupStudentAsync(inputGroupStudent))
                    .ReturnsAsync(updatedGroupStudent);

            //when
            GroupStudent actualGroupStudent=await
                this.groupStudentService.ModifyGroupStudentAsync(inputGroupStudent);  

            //then
            actualGroupStudent.Should().BeEquivalentTo(expectedGroupStudent);

          
            this.storageBrokerMock.Verify(broker=>
                broker.SelectGroupStudentByIdAsync(GroupId, StudentId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupStudentAsync(inputGroupStudent), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
