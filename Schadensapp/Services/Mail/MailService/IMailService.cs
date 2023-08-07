using Schadensapp.Models;
using Schadensapp.Models.Database;
using Schadensapp.Services.Mail.Model;

namespace Schadensapp.Services.Mail
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body);

        Task SendEmailAsyncWithAttachment(string email, string subject, string body, PDFModel model);
    }
}
