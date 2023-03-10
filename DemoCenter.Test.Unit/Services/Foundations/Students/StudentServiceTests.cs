using System;
using System.Linq;
using System.Linq.Expressions;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Students;
using DemoCenter.Services.Foundations.Students;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IStudentService studentService;
        public StudentServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.studentService = new StudentService(
                this.storageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();
        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 99).GetValue();
        private static Student CreateRandomStudent(DateTimeOffset dates) =>
            CreateStudentFiller(dates).Create();

        private static Student CreateRandomModifyStudent(DateTimeOffset dates)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Student randomStudent = CreateRandomStudent(dates);
            randomStudent.CreatedDate = randomStudent.CreatedDate.AddDays(randomDaysInPast);
            return randomStudent;
        }

        private static IQueryable<Student> CreateRandomStudents()
        {
            return CreateStudentFiller(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber()).AsQueryable();
        }
        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 99).GetValue();
        private static Student CreateRandomStudent() =>
            CreateStudentFiller(dates: GetRandomDateTimeOffset()).Create();

        private static Filler<Student> CreateStudentFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Student>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}
