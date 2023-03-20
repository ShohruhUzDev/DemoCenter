using System.Linq;
using DemoCenter.Models.Subjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllSubjects()
        {
            //given
            IQueryable<Subject> randomSubjects = CreateRandomSubjects();
            IQueryable<Subject> storageSubjects = randomSubjects;
            IQueryable<Subject> expectedSubjects = storageSubjects;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSubjects()).Returns(storageSubjects);

            //when
            IQueryable<Subject> actualSubjects =
                this.subjectService.RetrieveAllSubjects();

            //then
            actualSubjects.Should().BeEquivalentTo(expectedSubjects);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSubjects(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
