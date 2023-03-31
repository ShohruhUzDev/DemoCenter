using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Foundations.Users;
using DemoCenter.Services.Foundations.Users;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserService userService;

        public UserServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userService = new UserService(
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

        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
          actualException => actualException.SameExceptionAs(expectedException);

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static User CreateRandomUser(DateTimeOffset date) =>
        CreateUserFiller(date).Create();

        private static int GetRandomNumber() =>
           new IntRange(min: 2, max: 99).GetValue();

        private static int GetRandomNegativeNumber() =>
           -1 * new IntRange(min: 2, max: 10).GetValue();

        private static User CreateRandomModifyUser(DateTimeOffset dates)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            User randomUser = CreateRandomUser(dates);

            randomUser.CreatedDate = randomUser.CreatedDate.AddDays(randomDaysInPast);

            return randomUser;
        }

        private static IQueryable<User> CreateRandomUsers()
        {
            return CreateUserFiller(date: GetRandomDateTime())
                .Create(count: GetRandomNumber()).AsQueryable();
        }

        private static User CreateRandomUser() =>
         CreateUserFiller(GetRandomDateTime()).Create();

        private static Filler<User> CreateUserFiller(DateTimeOffset date)
        {
            var filler = new Filler<User>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date);

            return filler;
        }

    }
}
