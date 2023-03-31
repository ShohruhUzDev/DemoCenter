

using DemoCenter.Models.Foundations.Users;

namespace DemoCenter.Services.Foundations.Securities;

public interface ISecurityService
{
    string CreateToken(User user);
}
