using DemoCenter.Models.Subjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Subject> Subjects { get; set; }
        public async ValueTask<Subject> InsertSubjectAsync(Subject subject) =>
            await InsertAsync(subject);
        public async ValueTask<Subject> SelectSubjectByIdAsync(Guid id) =>
            await SelectAsync<Subject>(id); 
        public IQueryable<Subject> SelectAllSubjects() =>
            SelectAll<Subject>();
        public async ValueTask<Subject> UpdateSubjectAsync(Subject subject) =>
            await UpdateAsync(subject); 
        public async ValueTask<Subject> DeleteSubjectAsync(Subject subject) =>
            await DeleteAsync(subject);
    }
}
