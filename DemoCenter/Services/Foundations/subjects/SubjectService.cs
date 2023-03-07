using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Subjects;

namespace DemoCenter.Services.Foundations.Subjects
{
    public class SubjectService : ISubjectService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public SubjectService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Subject> AddSubjectAsync(Subject subject) =>
            await this.storageBroker.InsertSubjectAsync(subject);

        public IQueryable<Subject> RetrieveAllSubjects() =>
            this.storageBroker.SelectAllSubjects();

        public async ValueTask<Subject> RetrieveSubjectByIdAsync(Guid subjectId)=>
           await this.storageBroker.SelectSubjectByIdAsync(subjectId);
        public async ValueTask<Subject> RemoveSubjectByIdAsync(Guid subjectId)
        {
            Subject maybeSubject = await
                this.storageBroker.SelectSubjectByIdAsync(subjectId);

            return await this.storageBroker.DeleteSubjectAsync(maybeSubject);
        }
    }
}
