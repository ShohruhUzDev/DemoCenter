using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Teachers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public async Task ShouldRemoveSubjectByIdAsync()
        {
            //given
            Guid randomId = Guid.NewGuid();
            Guid inputTeacherId = randomId;
            Teacher randomTeacher = CreateRandomTeacher();
            Teacher storageTeacher = randomTeacher;
            Teacher expectedInputTeacher = storageTeacher;
            Teacher deletedTeacher = expectedInputTeacher;
            Teacher expectedTeacher = deletedTeacher.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(inputTeacherId)).ReturnsAsync(storageTeacher);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteTeacherAsync(expectedInputTeacher)).ReturnsAsync(deletedTeacher);

            //when
            Teacher actualTeacher = await
                this.teacherService.RemoveTeacherByIdAsync(inputTeacherId);

            //then
            actualTeacher.Should().BeEquivalentTo(expectedTeacher);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(inputTeacherId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTeacherAsync(expectedInputTeacher), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
