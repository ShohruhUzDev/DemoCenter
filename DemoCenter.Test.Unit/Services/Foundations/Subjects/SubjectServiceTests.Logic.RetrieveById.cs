using System;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveSubjectByIdAsync()
        {
            //given
            Guid randomSubjectId = Guid.NewGuid();
            Guid inputSubjectId = randomSubjectId;
            Subject randomSubject = CreateRandomSubject();
            Subject inputSubject = randomSubject;
            Subject storedSubject = inputSubject;
            Subject expectedSubject = storedSubject.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(inputSubjectId)).ReturnsAsync(storedSubject);

            //when
            Subject actualSubject = await
                this.subjectService.RetrieveSubjectByIdAsync(inputSubjectId);

            //then
            actualSubject.Should().BeEquivalentTo(expectedSubject);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(inputSubjectId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
