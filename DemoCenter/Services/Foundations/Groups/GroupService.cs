﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Foundations.Groups;

namespace DemoCenter.Services.Foundations.Groups
{
    public partial class GroupService : IGroupService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public GroupService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;

        }
        public ValueTask<Group> AddGroupAsync(Group group) =>
        TryCatch(async () =>
        {

            ValidateGroupOnAdd(group);

            return await this.storageBroker.InsertGroupAsync(group);
        });

        public IQueryable<Group> RetrieveAllGroups() =>
          TryCatch(() => this.storageBroker.SelectAllGroups());


        public ValueTask<Group> RetrieveGroupByIdAsync(Guid groupId) =>
            TryCatch(async () =>
            {
                ValidateGroupId(groupId);

                Group maybeGroup = await this.storageBroker.SelectGroupByIdAsync(groupId);
                ValidateStorageGroupExist(maybeGroup, groupId);

                return maybeGroup;
            });
        public ValueTask<Group> ModifyGroupAsync(Group group) =>
            TryCatch(async () =>
            {
                ValidateGroupOnModify(group);
                var maybeGroup = await this.storageBroker.SelectGroupByIdAsync(group.Id);
                ValidateAgainstStorageGroupOnModify(inputGroup: group, storageGroup: maybeGroup);

                return await this.storageBroker.UpdateGroupAsync(group);
            });

        public ValueTask<Group> RemoveGroupByIdAsync(Guid groupId) =>
            TryCatch(async () =>
            {
                ValidateGroupId(groupId);
                Group maybeGroup = await this.storageBroker.SelectGroupByIdAsync(groupId);
                ValidateStorageGroupExist(maybeGroup, groupId);

                return await this.storageBroker.DeleteGroupAsync(maybeGroup);
            });
    }
}