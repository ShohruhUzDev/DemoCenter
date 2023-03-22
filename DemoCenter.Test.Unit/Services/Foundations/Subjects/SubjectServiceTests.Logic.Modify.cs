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
        public async Task ShouldModifySubjectAsync()
        {
            //given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Subject randomSubject = CreateRandomModifySubjects(randomDate);
            Subject inputSubject = randomSubject;
            Subject storageSubject = inputSubject.DeepClone();
            storageSubject.UpdatedDate = randomSubject.CreatedDate;
            Subject updatedSubject = inputSubject;
            Subject expectedSubject = updatedSubject.DeepClone();
            Guid subjectid = inputSubject.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubjectByIdAsync(subjectid))
                    .ReturnsAsync(storageSubject);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateSubjectAsync(inputSubject)).ReturnsAsync(updatedSubject);

            //when
            Subject actualSubject = await this.subjectService.ModifySubjectAsync(inputSubject);

            //then
            actualSubject.Should().BeEquivalentTo(expectedSubject);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubjectByIdAsync(subjectid), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubjectAsync(inputSubject), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
