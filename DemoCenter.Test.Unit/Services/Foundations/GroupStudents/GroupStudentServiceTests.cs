using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Services.Foundations.GroupStudents;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly IGroupStudentService groupStudentService;

        public GroupStudentServiceTests()
        {
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.groupStudentService = new GroupStudentService(
                storageBroker: storageBrokerMock.Object,
                dateTimeBroker: dateTimeBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }

        public static TheoryData<int> InvalidSeconds()
        {
            int secondsInPast = -1 * new IntRange(
                min: 60,
                max: short.MaxValue).GetValue();

            int secondsInFuture = new IntRange(
                min: 0,
                max: short.MaxValue).GetValue();

            return new TheoryData<int>
            {
                secondsInPast,
                secondsInFuture
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();
        private static SqlException CreateSqlException() =>
          (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static GroupStudent CreateRandomModifyGroupStudent(DateTimeOffset dates)
        {
            int randomDaysAgo = GetRandomNegativeNumber();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(dates);

            randomGroupStudent.CreatedDate = randomGroupStudent.CreatedDate.AddDays(randomDaysAgo);

            return randomGroupStudent;
        }
        private static int GetRandomNegativeNumber() =>
           -1 * new IntRange(min: 2, max: 99).GetValue();

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static int GetRandomNumber() =>
               new IntRange(min: 2, max: 99).GetValue();
        private static IQueryable<GroupStudent> CreateRandomGroupStudents()
        {
            return CreateFillerGroupStudent(date: GetRandomDateTime())
                .Create(count: GetRandomNumber()).AsQueryable();
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
          actualException => actualException.SameExceptionAs(expectedException);

        private static GroupStudent CreateRandomGroupStudent(DateTimeOffset date) =>
            CreateFillerGroupStudent(date).Create();

        private static GroupStudent CreateRandomGroupStudent() =>
            CreateFillerGroupStudent(GetRandomDateTime()).Create();

        private static Filler<GroupStudent> CreateFillerGroupStudent(DateTimeOffset date)
        {
            var filler = new Filler<GroupStudent>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date);

            return filler;
        }

    }
}
