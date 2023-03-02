using System;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Students;
using DemoCenter.Services.Foundations.Students;
using Moq;
using Tynamix.ObjectFiller;

namespace DemoCenter.Test.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTest
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly IStudentService studentService;
        public StudentServiceTest()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();

            this.studentService = new StudentService(
                this.storageBrokerMock.Object);
        }

        private DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private Student CreateRandomStudent() =>
            CreateStudentFiller(dates: GetRandomDateTimeOffset()).Create();

        private Filler<Student> CreateStudentFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Student>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }

    }
}
