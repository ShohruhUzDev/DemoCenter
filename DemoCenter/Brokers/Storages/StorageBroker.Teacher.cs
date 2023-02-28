using DemoCenter.Models.Teachers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Teacher> Teachers { get; set; }

        public async ValueTask<Teacher> InsertTeacherAsync(Teacher teacher) =>
            await InsertAsync(teacher);

        public async ValueTask<Teacher> SelectTeacherByID(Guid id) =>
            await SelectAsync<Teacher>(id);

        public IQueryable<Teacher> SelectAllTeachers() =>
            SelectAll<Teacher>();

        public async ValueTask<Teacher> UpdateTeacherAsync(Teacher teacher) =>
            await UpdateAsync(teacher);

        public async ValueTask<Teacher> DeleteTeacherAsync(Teacher teacher) =>
            await DeleteAsync(teacher);
    }
}
