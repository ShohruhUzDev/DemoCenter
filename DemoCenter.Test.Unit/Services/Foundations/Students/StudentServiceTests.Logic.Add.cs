using System;
using System.Threading.Tasks;
using DemoCenter.Models.Students;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldAddStudentAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Student randomStudent = CreateRandomStudent(randomDateTime);
            Student inputStudent = randomStudent;
            Student persistedStudent = inputStudent;
            Student expectedStuden = persistedStudent.DeepClone();

            this.dateTimeBrokerMock.Setup(broker=>
                broker.GetCurrenDateTime()).Returns(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertStudentAsync(inputStudent)).ReturnsAsync(persistedStudent);

            //when
            Student actualStudent = await this.studentService.AddStudentAsync(inputStudent);


            //then
            actualStudent.Should().BeEquivalentTo(expectedStuden);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                 broker.InsertStudentAsync(inputStudent), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}