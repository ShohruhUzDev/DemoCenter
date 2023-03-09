using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Teachers;

namespace DemoCenter.Services.Foundations.Teachers
{
    public partial class TeacherService : ITeacherService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public TeacherService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Teacher> AddTeacherAsync(Teacher teacher) =>
            TryCatch(async () =>
            {
                ValidationTeacherOnAdd(teacher);
                return await storageBroker.InsertTeacherAsync(teacher);

            });


        public IQueryable<Teacher> RetrieveAllTeachers() =>
            this.storageBroker.SelectAllTeachers();

        public async ValueTask<Teacher> RetrieveTeacherByIdAsync(Guid teacherId) =>
           await this.storageBroker.SelectTeacherByIdAsync(teacherId);
        public async ValueTask<Teacher> ModifyTeacherAsync(Teacher teacher)
        {
            Teacher maybeTeacher =
                await this.storageBroker.SelectTeacherByIdAsync(teacher.Id);

            return await this.storageBroker.UpdateTeacherAsync(teacher);
        }
        public async ValueTask<Teacher> RemoveTeacherByIdAsync(Guid teacherid)
        {
            Teacher maybeTeacher = await
                   this.storageBroker.SelectTeacherByIdAsync(teacherid);

            return await this.storageBroker.DeleteTeacherAsync(maybeTeacher);
        }

    }
}