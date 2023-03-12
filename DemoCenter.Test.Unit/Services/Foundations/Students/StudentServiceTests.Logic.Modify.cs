using System;
using System.Threading.Tasks;
using DemoCenter.Models.Students;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldModifyStudentAsync()
        {
            //given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Student randomStudent = CreateRandomModifyStudent(randomDate);
            Student inputStudent = randomStudent;
            Student storageStudent = inputStudent.DeepClone();
            storageStudent.UpdatedDate = randomStudent.CreatedDate;
            Student updatedStudent = inputStudent;
            Student expectedStudent = updatedStudent.DeepClone();
            Guid studentId = inputStudent.Id;

            this.dateTimeBrokerMock.Setup(broker =>
            broker.GetCurrenDateTime()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
            broker.SelectStudentByIdAsync(studentId)).ReturnsAsync(storageStudent);

            this.storageBrokerMock.Setup(broker =>
            broker.UpdateStudentAsync(inputStudent)).ReturnsAsync(updatedStudent);

            //when
            Student actualStudent = await
                this.studentService.ModifyStudentAsync(inputStudent);

            //then
            actualStudent.Should().BeEquivalentTo(expectedStudent);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrenDateTime(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
            broker.SelectStudentByIdAsync(studentId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
            broker.UpdateStudentAsync(inputStudent), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
