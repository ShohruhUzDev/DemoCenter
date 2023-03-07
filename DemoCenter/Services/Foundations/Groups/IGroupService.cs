using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;

namespace DemoCenter.Services.Foundations.Groups
{
    public interface IGroupService
    {
        ValueTask<Group> AddGroupAsync(Group group);
        IQueryable<Group> RetrieveAllGroups();
        ValueTask<Group> RetrieveGroupByIdAsync(Guid groupId);
        ValueTask<Group> ModifyGroupAsync(Group group);
        ValueTask<Group> RemoveGroupByIdAsync(Guid groupId);
    }
}