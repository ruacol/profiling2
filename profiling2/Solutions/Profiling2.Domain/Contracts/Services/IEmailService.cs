
namespace Profiling2.Domain.Contracts.Services
{
    public interface IEmailService
    {
        void SendMailMessage(string[] recipients, string from, string subject, string body, bool isHtml);
    }
}
