using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Groups;
using DemoCenter.Services.Foundations.Groups;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Groups
{
    public partial class GroupServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly IGroupService groupService;

        public GroupServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();

            this.groupService = new GroupService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object,
                dateTimeBroker: dateTimeBrokerMock.Object);
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

        private IQueryable<Group> CreateRandomGroups()
        {
            return CreateGroupFiller(dates: GetRandomDateTimeOffset()).
                Create(count: GetRandomNumber()).AsQueryable();
        }
        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();
        private static Expression<Func<Xeption, bool>> SameExceptonAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
        private static Group CreateRandomGroup() =>
            CreateGroupFiller(dates: GetRandomDateTimeOffset()).Create();

        private static Group CreateRandomGroup(DateTimeOffset dates) =>
            CreateGroupFiller(dates).Create();

        private static int GetRandomNegativeNumber() =>
           -1 * new IntRange(min: 2, max: 99).GetValue();
        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 99).GetValue();

        private static Group CreateRandomModifyGroup(DateTimeOffset dates)
        {
            int randomDaysAgo = GetRandomNegativeNumber();
            Group randomGroup = CreateRandomGroup(dates);

            randomGroup.CreatedDate = randomGroup.CreatedDate.AddDays(randomDaysAgo);

            return randomGroup;
        }

        public static TheoryData MinutsBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }
        private static Filler<Group> CreateGroupFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Group>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }

    }
}
