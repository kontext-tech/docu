using System.Threading.Tasks;

namespace Kontext.Services
{
    public interface IEmailSenderService
    {
        Task<(bool success, string errorMsg)> SendEmailAsync(string email, string subject, string message);

        Task<(bool success, string errorMsg)> SendEmailAsync(string senderName, string senderEmail,
           string recepientName, string recepientEmail,
           string subject, string body, bool isHtml = true);

        Task<(bool success, string errorMsg)> SendEmailAsync(string recepientName, string recepientEmail,
            string subject, string body, bool isHtml = true);

        (string title, string body) BuildEmailContentFromTemplate(string templateName, (string key, string value)[] values, string languageLocale = null);

        Task<(bool success, string errorMsg)> SendEmailFromTemplateAsync(string email, string templateName, (string key, string value)[] values, string languageLocale = null);

        Task<(bool success, string errorMsg)> SendEmailFromTemplateAsync(string toUser, string toUserEmail, string bccEmail, string title, string templateName, (string key, string value)[] values, string languageLocale = null);
    }
}
