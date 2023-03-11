using System;
using System.Threading.Tasks;
using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidStudentId = Guid.Empty;
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.Id),
                values: "Id is required");
            
            var expectedGroupValidationException=new StudentValidationException(invalidStudentException);
            
            //when
            ValueTask<Student> onRemoveStudentTask=this.studentService.RemoveStudentByIdAsync(invalidStudentId);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(onRemoveStudentTask.AsTask);
          
            //then
            actualStudentValidationException.Should().BeEquivalentTo(expectedGroupValidationException);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(expectedGroupValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteStudentAsync(It.IsAny<Student>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();   
        }


    }
}
