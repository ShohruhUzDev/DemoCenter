using DemoCenter.Models.Subjects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCenter.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Subject> InsertSubjectAsync(Subject subject);
        ValueTask<Subject> SelectSubjectByIdAsync(Guid id);
        IQueryable<Subject> SelectAllSubjects();
        ValueTask<Subject> UpdateSubjectAsync(Subject subject);
        ValueTask<Subject> DeleteSubjectAsync(Subject subject);
    }
}
