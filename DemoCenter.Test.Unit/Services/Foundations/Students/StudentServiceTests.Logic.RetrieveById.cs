using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ShouldRetrieveStudentByIdAsync()
        {
            //given
            Guid randomStudentId= Guid.NewGuid();
            Guid inputStudentId = randomStudentId;
            Student randomStudent=CreateRandomStudent();
            Student storedStudent = randomStudent;
            Student  expectedStudent= storedStudent.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.SelectStudentByIdAsync(inputStudentId)).ReturnsAsync(storedStudent);

            //when
            Student actualStudent = await
                this.studentService.RetrieveStudentByIdAsync(inputStudentId);

            //then
            actualStudent.Should().BeEquivalentTo(expectedStudent);

            this.storageBrokerMock.Verify(broker =>
            broker.SelectStudentByIdAsync(inputStudentId), Times.Once); 

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
