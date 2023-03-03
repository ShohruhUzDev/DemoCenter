using System;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Subjects;
using DemoCenter.Services.Foundations.Subjects;
using Moq;
using Tynamix.ObjectFiller;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTest
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly ISubjectService subjectService;

        public SubjectServiceTest()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();

            this.subjectService = new SubjectService(
                storageBroker: this.storageBrokerMock.Object);
        }

        private DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private Subject CreateRandomSubject() =>
            CreateSubjectFiller(GetRandomDateTimeOffset()).Create();

        private Filler< Subject> CreateSubjectFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Subject>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}
