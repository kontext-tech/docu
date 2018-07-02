using Kontext.Services;
using System.Threading.Tasks;

namespace Kontext.Docu.Web.Portals.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSenderService emailSender, string email, string link, string languageLocale = null)
        {
            var template = emailSender.BuildEmailContentFromTemplate("Email.Register.Confirm", new(string key, string value)[] { ("link", link), ("email", email) }, languageLocale);
            return emailSender.SendEmailAsync(email, template.title, template.body);
        }
    }
}
