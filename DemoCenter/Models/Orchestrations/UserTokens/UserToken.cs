using System;

namespace DemoCenter.Models.Orchestrations.UserTokens
{
    public class UserToken
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
