using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Groups;

namespace DemoCenter.Services.Foundations.Groups
{
    public class GroupService : IGroupService
    {
        private IStorageBroker storageBroker;
        private IDateTimeBroker dateTimeBroker;
        private ILoggingBroker loggingBroker;

        public GroupService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;

        }
        public async ValueTask<Group> AddGroupAsync(Group group) =>
            await this.storageBroker.InsertGroupAsync(group);

        public IQueryable<Group> RetrieveAllGroups() =>
            this.storageBroker.SelectAllGroups();
        

        public ValueTask<Group> RetrieveGroupByIdAsync(Guid groupId) =>
            this.storageBroker.SelectGroupByIdAsync(groupId);
        
    }
}
