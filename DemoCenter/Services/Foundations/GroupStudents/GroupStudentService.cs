using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.GroupStudents;

namespace DemoCenter.Services.Foundations.GroupStudents
{
    public partial class GroupStudentService : IGroupStudentService
    {
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public GroupStudentService(
            ILoggingBroker loggingBroker,
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.loggingBroker = loggingBroker;
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<GroupStudent> AddGroupStudentAsync(GroupStudent student)
        {
            throw new NotImplementedException();
        }


        public IQueryable<GroupStudent> RetrieveAllGroupStudents()
        {
            throw new NotImplementedException();
        }

        public ValueTask<GroupStudent> RetrieveGroupStudentByIdAsync(Guid groupId, Guid studentId)
        {
            throw new NotImplementedException();
        }
        public ValueTask<GroupStudent> ModifyGroupStudentAsync(GroupStudent groupStudent)
        {
            throw new NotImplementedException();
        }

        public ValueTask<GroupStudent> RemoveGroupStudentByIdAsync(Guid groupId, Guid studentId)
        {
            throw new NotImplementedException();
        }
    }
}
