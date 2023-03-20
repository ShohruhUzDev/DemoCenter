using System;
using System.Linq;
using System.Net.Sockets;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Users;
using DemoCenter.Services.Foundations.Users;
using Moq;
using Tynamix.ObjectFiller;

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

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static User CreateRandomUser(DateTimeOffset date) =>
        CreateUserFiller(date).Create();

        private static int GetRandomNumber() =>
           new IntRange(min: 2, max: 99).GetValue();

        private static IQueryable<User> CreateRandomUsers()
        {
            return CreateUserFiller(date: GetRandomDateTime())
                .Create(count: GetRandomNumber()).AsQueryable();
        }

        private static User CreateRandomUser() =>
         CreateUserFiller(GetRandomDateTime()).Create();

        private static Filler<User> CreateUserFiller(DateTimeOffset date)
        {
            var filler= new Filler<User>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date);

            return filler;
        }

    }
}
