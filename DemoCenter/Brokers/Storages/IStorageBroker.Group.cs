using DemoCenter.Models.Groups;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCenter.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Group> InsertGroupAsync(Group group);
        ValueTask<Group> SelectGroupByIdAsync(Guid id);
        IQueryable<Group> SelectAllGroups();
        ValueTask<Group> UpdateGroupAsync(Group group);
        ValueTask<Group> DeleteGroupAsync(Group group);
    }
}
