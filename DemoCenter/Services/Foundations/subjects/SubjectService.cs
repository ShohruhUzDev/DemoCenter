using System.Threading.Tasks;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Subjects;

namespace DemoCenter.Services.Foundations.Subjects
{
    public class SubjectService : ISubjectService
    {
        private readonly IStorageBroker storageBroker;

        public SubjectService(IStorageBroker storageBroker) =>
              this.storageBroker = storageBroker;
        
        public ValueTask<Subject> AddSubjectAsync(Subject subject) =>
            throw new System.NotImplementedException();
        
    }
}
