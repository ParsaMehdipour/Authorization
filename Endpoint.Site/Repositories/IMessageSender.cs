using System.Threading.Tasks;

namespace Endpoint.Site.Repositories
{
    public interface IMessageSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false);
    }
}
