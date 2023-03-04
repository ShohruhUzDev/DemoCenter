using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;

namespace DemoCenter.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Group> InsertGroupAsync(Group group);
        IQueryable<Group> SelectAllGroups();
        ValueTask<Group> SelectGroupByIdAsync(Guid id);
        ValueTask<Group> UpdateGroupAsync(Group group);
        ValueTask<Group> DeleteGroupAsync(Group group);
    }
}
