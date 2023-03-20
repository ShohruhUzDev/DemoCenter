using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Users;

namespace DemoCenter.Services.Foundations.Users
{
    public class UserService : IUserService
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
            this.storageBroker.InsertUserAsync(user);              

        public IQueryable<User> RetrieveAllUsers() =>
            this.storageBroker.SelectAllUsers();
  
        public ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
            this.storageBroker.SelectUserByIdAsync(userId);

 
        public async ValueTask<User> ModifyUserAsync(User user)
        {
            User maybeUser=await this.storageBroker.SelectUserByIdAsync(user.Id);   

            return await this.storageBroker.UpdateUserAsync(maybeUser); 
        }

        public  async ValueTask<User> RemoveUserByIdAsync(Guid userId)
        {
            User maybeUser = await this.storageBroker.SelectUserByIdAsync(userId);

            return await this.storageBroker.DeleteUserAsync(maybeUser);
        }
    }
}
