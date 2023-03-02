using System.Threading.Tasks;
using DemoCenter.Models.Groups;

namespace DemoCenter.Services.Foundations.Groups
{
    public interface IGroupService
    {
        ValueTask<Group> AddGroupAsync(Group group);
    }
}