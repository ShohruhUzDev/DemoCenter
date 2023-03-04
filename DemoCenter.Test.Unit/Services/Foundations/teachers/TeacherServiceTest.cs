using System;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Teachers;
using DemoCenter.Services.Foundations.Teachers;
using Moq;
using Tynamix.ObjectFiller;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTest
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly TeacherService teacherService;

        public TeacherServiceTest()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();

            this.teacherService = new TeacherService(
                storageBroker: this.storageBrokerMock.Object);
        }

        private DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private Teacher CreateRandomTeacher() =>
            CreateRandomFiller(GetRandomDateTimeOffset()).Create();

        private Filler<Teacher> CreateRandomFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Teacher>();

            filler.Setup()
                 .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }


    }
}
