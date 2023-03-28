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
        public async Task ShouldAddGroupStudentAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(randomDateTime);
            GroupStudent inputGroupStudent = randomGroupStudent;
            GroupStudent persistedGroupStudent = inputGroupStudent;
            GroupStudent expectedGroupStudent = persistedGroupStudent.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertGroupStudentAsync(inputGroupStudent))
                    .ReturnsAsync(persistedGroupStudent);

            //when
            GroupStudent actualGroupStudent =
                await this.groupStudentService.AddGroupStudentAsync(inputGroupStudent);

            //then
            actualGroupStudent.Should().BeEquivalentTo(expectedGroupStudent);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(inputGroupStudent), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
