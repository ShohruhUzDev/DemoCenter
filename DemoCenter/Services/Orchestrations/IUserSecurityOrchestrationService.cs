
using DemoCenter.Models.Orchestrations.UserTokens;

namespace DemoCenter.Services.Orchestrations
{
    public interface IUserSecurityOrchestrationService
    {
        UserToken CreateUserToken(string email, string password);
    }
}
