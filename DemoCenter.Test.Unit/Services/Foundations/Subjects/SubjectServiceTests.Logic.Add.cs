using System;
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
            DateTimeOffset randomDate=GetRandomDateTimeOffset();
            Subject randomSubject = CreateRandomSubject(randomDate);
            Subject inputSubject = randomSubject;
            Subject persistedSubject = inputSubject;
            Subject expectedSubject = persistedSubject.DeepClone();

            this.dateTimeBrokerMock.Setup(broker=>
                broker.GetCurrenDateTime()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSubjectAsync(inputSubject)).ReturnsAsync(persistedSubject);

            //when
            Subject actualSubject = await subjectService.AddSubjectAsync(inputSubject);

            //then
            actualSubject.Should().BeEquivalentTo(expectedSubject);

            this.dateTimeBrokerMock.Verify(broker=>
                broker.GetCurrenDateTime(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubjectAsync(inputSubject), Times.Once);

            storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
