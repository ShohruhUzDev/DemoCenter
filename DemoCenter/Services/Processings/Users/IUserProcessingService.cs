


using DemoCenter.Models.Foundations.Users;

namespace DemoCenter.Services.Processings.Users
{
    public interface IUserProcessingService
    {
        User RetrieveUserByCredentails(string email, string password);
    }
}