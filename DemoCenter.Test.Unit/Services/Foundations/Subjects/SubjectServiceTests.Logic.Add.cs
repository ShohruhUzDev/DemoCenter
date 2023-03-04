using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    partial class SubjectServiceTests
    {
        [Fact]
        public async Task ShouldAddSubjectAsync()
        {
            //given
            Subject randomSubject = CreateRandomSubject();
            Subject inputSubject = randomSubject;
            Subject persistedSubject = inputSubject;
            Subject expectedSubject = persistedSubject.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.InsertSubjectAsync(inputSubject)).ReturnsAsync(persistedSubject);

            //when
            Subject actualSubject = await subjectService.AddSubjectAsync(inputSubject);

            //then
            actualSubject.Should().BeEquivalentTo(expectedSubject);

            this.storageBrokerMock.Verify(broker =>
            broker.InsertSubjectAsync(inputSubject), Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
