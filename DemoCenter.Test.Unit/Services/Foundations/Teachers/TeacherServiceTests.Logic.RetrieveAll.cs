using System.Linq;
using DemoCenter.Models.Teachers;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllTeachers()
        {
            //given
            IQueryable<Teacher> randomTeachers = CreateRandomTeachers();
            IQueryable<Teacher> storageTeachers = randomTeachers;
            IQueryable<Teacher> expectedTeachers = storageTeachers;

            this.storageBrokerMock.Setup(broker =>
            broker.SelectAllTeachers()).Returns(storageTeachers);

            //when
            IQueryable<Teacher> actualTeachers =
                this.teacherService.RetrieveAllTeachers();

            //then
            actualTeachers.Should().BeEquivalentTo(expectedTeachers);

            this.storageBrokerMock.Verify(broker =>
            broker.SelectAllTeachers(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();

        }
    }
}
