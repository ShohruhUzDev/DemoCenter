using System.Threading.Tasks;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Teachers;

namespace DemoCenter.Services.Foundations.Teachers
{
    public class TeacherService : ITeacherService
    {
        private readonly IStorageBroker storageBroker;

        public TeacherService(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        public async ValueTask<Teacher> AddTeacherAsync(Teacher teacher) =>
            await storageBroker.InsertTeacherAsync(teacher);
    }
}