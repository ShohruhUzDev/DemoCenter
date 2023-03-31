using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Groups;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Group> Groups { get; set; }
        public async ValueTask<Group> InsertGroupAsync(Group group) =>
            await InsertAsync(group);
        public IQueryable<Group> SelectAllGroups() =>
            SelectAll<Group>();
        public async ValueTask<Group> SelectGroupByIdAsync(Guid id) =>
            await SelectAsync<Group>(id);
        public async ValueTask<Group> UpdateGroupAsync(Group group) =>
            await UpdateAsync(group);
        public async ValueTask<Group> DeleteGroupAsync(Group group) =>
            await DeleteAsync(group);

    }
}
