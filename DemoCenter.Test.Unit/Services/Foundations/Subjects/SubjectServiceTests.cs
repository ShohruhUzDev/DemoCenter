using System;
using System.Linq;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Subjects;
using DemoCenter.Services.Foundations.Subjects;
using Moq;
using Tynamix.ObjectFiller;

namespace DemoCenter.Test.Unit.Services.Foundations.Subjects
{
    public partial class SubjectServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ISubjectService subjectService;

        public SubjectServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.subjectService = new SubjectService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private IQueryable<Subject> CreateRandomSubjects()
        {
            return CreateSubjectFiller(dates:GetRandomDateTimeOffset())
                .Create(count:GetRandomNumber()).AsQueryable();
        }
        private static int GetRandomNumber()=>
            new IntRange(min:2, max:99).GetValue();
        private Subject CreateRandomSubject() =>
            CreateSubjectFiller(GetRandomDateTimeOffset()).Create();

        private Filler<Subject> CreateSubjectFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Subject>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}
