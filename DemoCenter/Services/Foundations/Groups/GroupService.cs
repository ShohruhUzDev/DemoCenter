﻿using System.Reflection.Metadata.Ecma335;
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

        public async ValueTask<Group> AddGroupAsync(Group group) =>
            await this.storageBroker.InsertGroupAsync(group);
        
    }
}