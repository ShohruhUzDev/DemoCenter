using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Students;

namespace DemoCenter.Services.Foundations.Students
{
    public partial class StudentService : IStudentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public StudentService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;

        }
        public ValueTask<Student> AddStudentAsync(Student student) =>
            TryCatch(async () =>
            {
                ValidateStudentOnAdd(student);

                return await this.storageBroker.InsertStudentAsync(student);
            });


        public IQueryable<Student> RetrieveAllStudents() =>
            this.storageBroker.SelectAllStudents();

        public ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId) =>
            this.storageBroker.SelectStudentByIdAsync(studentId);

        public ValueTask<Student> ModifyStudentAsync(Student student) =>
            TryCatch(async () =>
            {
                ValidateStudentOnModify(student);

                Student maybeStudent = await
                       this.storageBroker.SelectStudentByIdAsync(student.Id);

                return await this.storageBroker.UpdateStudentAsync(student);
            });

        public ValueTask<Student> RemoveStudentByIdAsync(Guid studentId) =>
            TryCatch(async () =>
            {
                ValidateStudentId(studentId);
                Student maybeStudent =
                    await this.storageBroker.SelectStudentByIdAsync(studentId);
                return await this.storageBroker.DeleteStudentAsync(maybeStudent);

            });
    }
}