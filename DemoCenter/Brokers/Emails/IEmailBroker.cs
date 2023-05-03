using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Emails;
using PostmarkDotNet;

namespace DemoCenter.Brokers.Emails
{
    public interface IEmailBroker
    {
        Task<PostmarkResponse> SendEmailAsync(Email email);
    }
}
