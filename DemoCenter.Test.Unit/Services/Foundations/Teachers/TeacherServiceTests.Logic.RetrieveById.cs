using System;
using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveTeacherByIdAsync()
        {
            //given
            Guid randomTeacherId = Guid.NewGuid();
            Guid inputTeacherId = randomTeacherId;
            Teacher inputTeacher = CreateRandomTeacher();
            Teacher storedTeacher = inputTeacher;
            Teacher expectedTeacher = storedTeacher.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.SelectTeacherByIdAsync(inputTeacherId)).ReturnsAsync(storedTeacher);

            //when
            Teacher actualTeacher = await
                this.teacherService.RetrieveTeacherByIdAsync(inputTeacherId);

            //then
            actualTeacher.Should().BeEquivalentTo(expectedTeacher);

            this.storageBrokerMock.Verify(broker =>
            broker.SelectTeacherByIdAsync(inputTeacherId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
