﻿using System;
using System.Linq;
using System.Linq.Expressions;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Teachers;
using DemoCenter.Services.Foundations.Teachers;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ITeacherService teacherService;

        public TeacherServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.teacherService = new TeacherService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static Teacher CreateRandomTeacher(DateTimeOffset dates) =>
            CreateTeacherFiller(dates).Create();

        private static Teacher CreateRandomModifyTeacher(DateTimeOffset dates)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Teacher randomTeacher = CreateRandomTeacher(dates);

            randomTeacher.CreatedDate = randomTeacher.CreatedDate
                .AddDays(randomDaysInPast);

            return randomTeacher;
        }

        private IQueryable<Teacher> CreateRandomTeachers()
        {
            return CreateTeacherFiller(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber()).AsQueryable();
        }
        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 99).GetValue();
        private Teacher CreateRandomTeacher() =>
            CreateTeacherFiller(GetRandomDateTimeOffset()).Create();

        private static Filler<Teacher> CreateTeacherFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Teacher>();

            filler.Setup()
                 .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }


    }
}
