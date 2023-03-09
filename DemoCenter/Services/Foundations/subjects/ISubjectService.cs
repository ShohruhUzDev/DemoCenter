using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects;

namespace DemoCenter.Services.Foundations.Subjects
{
    public interface ISubjectService
    {
        ValueTask<Subject> AddSubjectAsync(Subject subject);
        IQueryable<Subject> RetrieveAllSubjects();
        ValueTask<Subject> RetrieveSubjectByIdAsync(Guid subjectId);
        ValueTask<Subject> ModifySubjectAsync(Subject subject);
        ValueTask<Subject> RemoveSubjectByIdAsync(Guid subjectId);
    }
}