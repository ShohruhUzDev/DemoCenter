using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Students;
using DemoCenter.Models.Subjects;
using DemoCenter.Services.Foundations.Subjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

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

        private Subject CreateRandomModifySubjects(DateTimeOffset date)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Subject randomSubject = CreateRandomSubject(date);
            randomSubject.CreatedDate = randomSubject.CreatedDate.AddDays(randomDaysInPast);
            return randomSubject;
        }
        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private IQueryable<Subject> CreateRandomSubjects()
        {
            return CreateSubjectFiller(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber()).AsQueryable();
        }

        private static int GetRandomNegativeNumber() =>
          -1* new IntRange(min: 2, max: 99).GetValue();
        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 99).GetValue();

        private Subject CreateRandomSubject(DateTimeOffset date) =>
            CreateSubjectFiller(date).Create();

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
