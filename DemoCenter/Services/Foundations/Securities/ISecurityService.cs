//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Tarteeb.Api.Models.Foundations.Users;

namespace Tarteeb.Api.Services.Foundations.Securities;

public interface ISecurityService
{
    string CreateToken(User user);
}
