
using System.Linq;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Models.Foundations.Users;
using DemoCenter.Services.Foundations.Users;

namespace DemoCenter.Services.Processings.Users
{
    public partial class UserProcessingService : IUserProcessingService
    {
        private readonly IUserService userService;
        private readonly ILoggingBroker loggingBroker;

        public UserProcessingService(IUserService userService, ILoggingBroker loggingBroker)
        {
            this.userService = userService;
            this.loggingBroker = loggingBroker;
        }

        public User RetrieveUserByCredentails(string email, string password) =>
        TryCatch(() =>
        {
            ValidateEmailAndPassword(email, password);
            IQueryable<User> allUser = this.userService.RetrieveAllUsers();

            return allUser.FirstOrDefault(retrievedUser => retrievedUser.Email.Equals(email)
                    && retrievedUser.Password.Equals(password));
        });
    }
}
