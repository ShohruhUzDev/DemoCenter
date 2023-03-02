using System.Threading.Tasks;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Students;

namespace DemoCenter.Services.Foundations.Students
{
    public class StudentService : IStudentService
    {
        private readonly IStorageBroker storageBroker;

        public StudentService(IStorageBroker storageBroker) =>
                 this.storageBroker = storageBroker;

        public ValueTask<Student> AddStudentAsync(Student student) =>
            this.storageBroker.InsertStudentAsync(student);
    }
}