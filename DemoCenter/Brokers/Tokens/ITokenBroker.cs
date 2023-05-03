
using DemoCenter.Models.Foundations.Users;

namespace DemoCenter.Brokers.Tokens
{
    public interface ITokenBroker
    {
        string GenerateJWT(User user);
        string HashToken(string password);
    }
}
