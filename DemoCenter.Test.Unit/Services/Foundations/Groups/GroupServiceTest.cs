using System;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Groups;
using DemoCenter.Services.Foundations.Groups;
using Moq;
using Tynamix.ObjectFiller;

namespace DemoCenter.Test.Services.Foundations.Groups
{
    partial class GroupServiceTest
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly IGroupService groupService;

        public GroupServiceTest()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();

            this.groupService = new GroupService(
                storageBroker: this.storageBrokerMock.Object);
        }
        private DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private Group CreateRandomGroup() =>
            CreateGroupFiller(dates: GetRandomDateTimeOffset()).Create();

        private Filler<Group> CreateGroupFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Group>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}