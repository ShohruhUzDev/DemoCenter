using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Tokens;
using DemoCenter.Models.Foundations.Users;

namespace DemoCenter.Services.Foundations.Securities;

public partial class SecurityService : ISecurityService
{
    private readonly ITokenBroker tokenBroker;
    private readonly ILoggingBroker loggingBroker;

    public SecurityService(
        ITokenBroker tokenBroker,
        ILoggingBroker loggingBroker)
    {
        this.tokenBroker = tokenBroker;
        this.loggingBroker = loggingBroker;
    }

    public string CreateToken(User user) =>
    TryCatch(() =>
    {
        ValidateUser(user);

        return tokenBroker.GenerateJWT(user);
    });



}
