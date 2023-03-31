using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Subjects;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTests
    {
        [Fact]
        public async Task ShouldRemoveSubjectByIdAsync()
        {
            //given
            Guid randomSubjectId = Guid.NewGuid();
            Guid inputSubjectId = randomSubjectId;
            Subject randomSubject = CreateRandomSubject();
            Subject storageSubject = randomSubject;
            Subject expectedInputSubject = storageSubject;
            Subject deletedSubject = expectedInputSubject;
            Subject expectedSubject = deletedSubject.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(inputSubjectId)).ReturnsAsync(storageSubject);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteSubjectAsync(expectedInputSubject)).ReturnsAsync(deletedSubject);

            //when
            Subject actualSubject = await
                this.subjectService.RemoveSubjectByIdAsync(inputSubjectId);

            //then
            actualSubject.Should().BeEquivalentTo(expectedSubject);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(inputSubjectId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubjectAsync(expectedInputSubject), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
