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
        public async Task ShouldRemoveStudentByIdAsync()
        {
            //given
            Guid randomStudentId= Guid.NewGuid();
            Guid inputStudentId = randomStudentId;
            Student randomStudent=CreateRandomStudent();
            Student storageStudent = randomStudent;
            Student expectedInputStudent = storageStudent;
            Student deletedStudent = expectedInputStudent;
            Student expectedStudent = deletedStudent.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.SelectStudentByIdAsync(inputStudentId)).ReturnsAsync(storageStudent);

            this.storageBrokerMock.Setup(broker =>
            broker.DeleteStudentAsync(expectedInputStudent)).ReturnsAsync(deletedStudent);

            //when
            Student actualStudent=await this.studentService.RemoveStudentByIdAsync(inputStudentId);

            //then
            actualStudent.Should().BeEquivalentTo(expectedStudent);

            this.storageBrokerMock.Verify(broker =>
            broker.SelectStudentByIdAsync(inputStudentId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
            broker.DeleteStudentAsync(expectedInputStudent), Times.Once()); 

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();   
            this.loggingBrokerMock.VerifyNoOtherCalls();    
        }
    }
}
