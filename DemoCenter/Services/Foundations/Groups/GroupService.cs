using System.Threading.Tasks;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Groups;

namespace DemoCenter.Services.Foundations.Groups
{
    public class GroupService : IGroupService
    {
        private IStorageBroker storageBroker;

        public GroupService(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        public ValueTask<Group> AddGroupAsync(Group group) =>
            throw new System.NotImplementedException();
        
    }
}
