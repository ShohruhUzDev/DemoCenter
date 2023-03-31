using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.GroupStudents;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<GroupStudent> GroupStudents { get; set; }
        public async ValueTask<GroupStudent> InsertGroupStudentAsync(GroupStudent groupStudent) =>
            await InsertAsync(groupStudent);
        public IQueryable<GroupStudent> SelectAllGroupStudents() =>
            SelectAll<GroupStudent>();
        public async ValueTask<GroupStudent> SelectGroupStudentByIdAsync(Guid groupId, Guid studentId) =>
            await SelectAsync<GroupStudent>(groupId, studentId);
        public async ValueTask<GroupStudent> UpdateGroupStudentAsync(GroupStudent groupStudent) =>
            await UpdateAsync(groupStudent);
        public async ValueTask<GroupStudent> DeleteGroupStudentAsync(GroupStudent groupStudent) =>
            await DeleteAsync(groupStudent);
    }
}
