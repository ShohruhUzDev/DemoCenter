﻿using System;
using System.Linq;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Services.Foundations.GroupStudents;
using Moq;
using Tynamix.ObjectFiller;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly IGroupStudentService groupStudentService;

        public GroupStudentTests()
        {
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.groupStudentService = new GroupStudentService(
                storageBroker: storageBrokerMock.Object,
                dateTimeBroker: dateTimeBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static int GetRandomNumber() =>
               new IntRange(min: 2, max: 99).GetValue();
        private static IQueryable<GroupStudent> CreateRandomGroupStudents()
        {
            return CreateFillerGroupStudent(date: GetRandomDateTime())
                .Create(count: GetRandomNumber()).AsQueryable();
        }

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
