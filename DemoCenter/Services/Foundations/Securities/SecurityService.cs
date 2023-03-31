//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Brokers.Tokens;
using Tarteeb.Api.Models.Foundations.Users;

namespace Tarteeb.Api.Services.Foundations.Securities;

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
