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
        public async Task ShouldModifyTeacherAsync()
        {
            //given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Teacher randomTeacher = CreateRandomModifyTeacher(randomDate);
            Teacher inputTeacher = randomTeacher;
            Teacher storageTeacher = inputTeacher.DeepClone();
            storageTeacher.UpdatedDate = randomTeacher.CreatedDate;
            Teacher updatedTeacher = inputTeacher;
            Teacher expectedTeacher = updatedTeacher.DeepClone();
            Guid teacherId = inputTeacher.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                 broker.GetCurrenDateTime()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTeacherByIdAsync(teacherId))
                    .ReturnsAsync(storageTeacher);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateTeacherAsync(inputTeacher))
                    .ReturnsAsync(updatedTeacher);

            //when
            Teacher actualTeacher =
                await this.teacherService.ModifyTeacherAsync(inputTeacher);

            //then
            actualTeacher.Should().BeEquivalentTo(expectedTeacher);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTeacherByIdAsync(teacherId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTeacherAsync(inputTeacher), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
