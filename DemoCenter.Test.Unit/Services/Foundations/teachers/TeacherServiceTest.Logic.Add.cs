using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    partial class TeacherServiceTest
    {
        [Fact]
        public async Task ShouldAddTeacherAsync()
        {
            //given
            Teacher randomTeacher = CreateRandomTeacher();
            Teacher inputTeacher = randomTeacher;
            Teacher persistedTeacher = inputTeacher;
            Teacher expectedTeacher = persistedTeacher.DeepClone();


            this.storageBrokerMock.Setup(broker =>
            broker.InsertTeacherAsync(inputTeacher)).ReturnsAsync(persistedTeacher);

            //when
            Teacher actualTeacher = await this.teacherService.AddTeacherAsync(inputTeacher);

            //then
            actualTeacher.Should().NotBeEquivalentTo(expectedTeacher);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertTeacherAsync(inputTeacher), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
