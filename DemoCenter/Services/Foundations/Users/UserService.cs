﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Foundations.Users;

namespace DemoCenter.Services.Foundations.Users
{
    public partial class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserService(
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            IStorageBroker storageBroker)
        {
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.storageBroker = storageBroker;
        }

        public ValueTask<User> AddUserAsync(User user) =>
        TryCatch(async () =>
        {
            ValidateUserOnAdd(user);
            return await this.storageBroker.InsertUserAsync(user);

        });

        public IQueryable<User> RetrieveAllUsers() =>
        TryCatch(() =>
        {
            return this.storageBroker.SelectAllUsers();

        });

        public ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
        TryCatch(async () =>
        {
            ValidateUserId(userId);
            User maybeUser = await this.storageBroker.SelectUserByIdAsync(userId);
            ValidateStorageUser(maybeUser, userId);

            return maybeUser;
        });


        public ValueTask<User> ModifyUserAsync(User user) =>
        TryCatch(async () =>
        {
            ValidateUserOnModify(user);
            User maybeUser = await this.storageBroker.SelectUserByIdAsync(user.Id);
            ValidateAginstStorageUserOnModify(user, maybeUser);
            return await this.storageBroker.UpdateUserAsync(user); ;
        });


        public ValueTask<User> RemoveUserByIdAsync(Guid userId) =>
        TryCatch(async () =>
        {
            ValidateUserId(userId);
            User maybeUser = await this.storageBroker.SelectUserByIdAsync(userId);
            ValidateStorageUser(maybeUser, userId);
            return await this.storageBroker.DeleteUserAsync(maybeUser);
        });
    }
}
