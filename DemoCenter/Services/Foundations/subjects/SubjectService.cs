using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Subjects;

namespace DemoCenter.Services.Foundations.Subjects
{
    public partial class SubjectService : ISubjectService
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

        public ValueTask<Subject> AddSubjectAsync(Subject subject) =>
        TryCatch(async () =>
        {
            ValidateSubjectOnAdd(subject);

            return await this.storageBroker.InsertSubjectAsync(subject);

        });

        public IQueryable<Subject> RetrieveAllSubjects() =>
            TryCatch(() =>this.storageBroker.SelectAllSubjects());

        public ValueTask<Subject> RetrieveSubjectByIdAsync(Guid subjectId) =>
            TryCatch(async () =>
            {
                ValidateSubjectId(subjectId);
                Subject maybeSubject = await this.storageBroker.SelectSubjectByIdAsync(subjectId);
                ValidatStorageSubjectExist(maybeSubject, subjectId);

                return maybeSubject;
            });

        public ValueTask<Subject> ModifySubjectAsync(Subject subject) =>
            TryCatch(async () =>
            {
                ValidateSubjectOnModify(subject);
                Subject maybeSubject = await
                       this.storageBroker.SelectSubjectByIdAsync(subject.Id);

                ValidateAginstStorageSubjectOnModify(inputSubject: subject, storageSubject: maybeSubject);

                return await this.storageBroker.UpdateSubjectAsync(subject);
            });

        public ValueTask<Subject> RemoveSubjectByIdAsync(Guid subjectId) =>
            TryCatch(async () =>
            {
                ValidateSubjectId(subjectId);
                Subject maybeSubject = await
                    this.storageBroker.SelectSubjectByIdAsync(subjectId);

                ValidatStorageSubjectExist(maybeSubject, subjectId);

                return await this.storageBroker.DeleteSubjectAsync(maybeSubject);
            });
    }
}
