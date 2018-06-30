using Kontext.Data;
using Kontext.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Kontext.Data.Models;

namespace Kontext.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfigService configService;
        private readonly IContextUnitOfWork unitOfWork;
        private readonly ILoggerFactory loggerFactory;
        private readonly IFileProvider fileProvider;
        private readonly IConfiguration rootConfig;

        public EmailSenderService(IConfigService configService, IContextUnitOfWork unitOfWork, ILoggerFactory loggerFactory, IFileProvider fileProvider, IConfiguration rootConfig)
        {
            this.configService = configService;
            this.unitOfWork = unitOfWork;
            this.loggerFactory = loggerFactory;
            this.fileProvider = fileProvider;
            this.rootConfig = rootConfig;
        }

        /// <summary>
        /// Send email from template
        /// </summary>
        /// <param name="email"></param>
        /// <param name="templateName"></param>
        /// <param name="values"></param>
        /// <param name="languageLocale"></param>
        /// <returns></returns>
        public Task<(bool success, string errorMsg)> SendEmailFromTemplateAsync(string email, string templateName, (string key, string value)[] values, string languageLocale = null)
        {
            var emailTitleAndBody = BuildEmailContentFromTemplate(templateName, values, languageLocale);
            return SendEmailAsync(email, emailTitleAndBody.title, emailTitleAndBody.body);
        }

        /// <summary>
        /// Send email with more params 
        /// </summary>
        /// <param name="toUser"></param>
        /// <param name="toUserEmail"></param>
        /// <param name="bccEmail"></param>
        /// <param name="title"></param>
        /// <param name="templateName"></param>
        /// <param name="values"></param>
        /// <param name="languageLocale"></param>
        /// <returns></returns>
        public Task<(bool success, string errorMsg)> SendEmailFromTemplateAsync(string toUser, string toUserEmail, string bccEmail, string title, string templateName, (string key, string value)[] values, string languageLocale = null)
        {
            var emailTitleAndBody = BuildEmailContentFromTemplate(templateName, values, languageLocale);
            // MailAddress sender, MailAddress[] recepients, string subject, string body, bool isHtml = true, MailAddress[] bccList = null
            var sender = new MailAddress(configService.EmailConfig.FromEmail, configService.EmailConfig.FromEmailDisplayName);
            var recepients = new MailAddress[] { new MailAddress(toUserEmail, toUser) };
            MailAddress[] bccList = null;

            if (bccEmail != null)
            {
                bccList = new MailAddress[] { new MailAddress(bccEmail, bccEmail) };
            }
            return SendEmailAsync(sender: sender, recepients: recepients, subject: $"{emailTitleAndBody.title} - {title}", body: emailTitleAndBody.body, bccList: bccList);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<(bool success, string errorMsg)> SendEmailAsync(string email, string subject, string message)
        {
            return await SendEmailAsync(email, email, subject, message, true);
        }

        /// <summary>
        /// Cached templates
        /// </summary>
        static IDictionary<string, string> CachedTemplates;

        static EmailSenderService()
        {
            CachedTemplates = new Dictionary<string, string>();
        }

        /// <summary>
        /// Check whether the email template is already cached. 
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        private static bool CheckIsTemplateCached(string templateName)
        {
            return CachedTemplates.ContainsKey(templateName);
        }

        /// <summary>
        /// Send email 
        /// </summary>
        /// <param name="recepientName"></param>
        /// <param name="recepientEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public async Task<(bool success, string errorMsg)> SendEmailAsync(string recepientName, string recepientEmail,
            string subject, string body, bool isHtml = true)
        {
            var from = new MailAddress(configService.EmailConfig.FromEmail, configService.EmailConfig.FromEmailDisplayName);
            var to = new MailAddress(recepientEmail, recepientName);

            return await SendEmailAsync(from, new MailAddress[] { to }, subject, body, isHtml);
        }

        /// <summary>
        /// Send email 
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        /// <param name="recepientName"></param>
        /// <param name="recepientEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public async Task<(bool success, string errorMsg)> SendEmailAsync(string senderName, string senderEmail,
            string recepientName, string recepientEmail,
            string subject, string body, bool isHtml = true)
        {
            var from = new MailAddress(senderEmail, senderName);
            var to = new MailAddress(recepientEmail, recepientName);

            return await SendEmailAsync(from, new MailAddress[] { to }, subject, body, isHtml);
        }

        /// <summary>
        /// Send email 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="recepients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<(bool success, string errorMsg)> SendEmailAsync(MailAddress sender, MailAddress[] recepients, string subject, string body, bool isHtml = true, MailAddress[] bccList = null)
        {
            MailMessage message = new MailMessage
            {
                From = sender
            };

            message.Subject = subject;
            message.IsBodyHtml = isHtml;
            message.Body = body;
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;
            foreach (var recepient in recepients)
                message.To.Add(recepient);

            if (bccList != null)
            {
                foreach (var recepient in recepients)
                    message.Bcc.Add(recepient);
            }

            var emailMessage = AddEmailMessageToStore(message);
            try
            {
                using (var client = new SmtpClient())
                {
                    var config = configService.EmailConfig;

                    if (config.EnableSSl)
                        client.EnableSsl = true;
                    client.Host = config.Host;
                    client.Port = config.Port;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Credentials = new NetworkCredential(config.UserName, rootConfig[config.PasswordConfigurationName]);

                    await client.SendMailAsync(message);
                    emailMessage.IsSent = true;
                    emailMessage.IsSuccessful = true;
                    emailMessage.DateSent = DateTime.Now;
                    emailMessage.DateModified = DateTime.Now;
                    SaveEmailMessage(emailMessage);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                if (loggerFactory != null)
                    loggerFactory.CreateLogger<EmailSenderService>().LogError(LoggingEvents.SEND_EMAIL_ERROR, ex, "An error occurred while sending email");
                emailMessage.IsSent = false;
                emailMessage.IsSuccessful = false;
                emailMessage.Comment = ex.ToString();
                emailMessage.DateModified = DateTime.Now;
                SaveEmailMessage(emailMessage);

                return (false, ex.Message);
            }

        }

        /// <summary>
        /// Save updates
        /// </summary>
        /// <param name="msg"></param>
        private void SaveEmailMessage(EmailMessage msg)
        {
            unitOfWork.EmailMessageRepository.Update(msg);
            unitOfWork.SaveChanges();
        }

        /// <summary>
        /// Add message to database store
        /// </summary>
        /// <param name="msg"></param>
        private EmailMessage AddEmailMessageToStore(MailMessage msg)
        {
            var emailMessage = new EmailMessage
            {
                Title = msg.Subject,
                MessageBody = msg.Body,
                FromEmail = msg.From.Address,
                FromEmailDisplayName = msg.From.DisplayName,
                IsSent = false,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };
            if (msg.To != null)
            {
                emailMessage.ToEmails = string.Join(";", msg.To.Select(a => a.Address));
            }

            if (msg.CC != null)
            {
                emailMessage.CCEmails = string.Join(";", msg.CC.Select(a => a.Address));
            }

            unitOfWork.EmailMessageRepository.Add(emailMessage);
            unitOfWork.SaveChanges();
            return emailMessage;
        }

        /// <summary>
        /// Build email content from template.
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="values"></param>
        /// <param name="languageLocale"></param>
        /// <returns></returns>
        public (string title, string body) BuildEmailContentFromTemplate(string templateName, (string key, string value)[] values, string languageLocale = null)
        {
            var templateConfig = (from template in configService.EmailTemplates
                                  where template.TemplateName == templateName && ((languageLocale ?? "") == (template.TemplateLanguageLocale ?? "") || string.IsNullOrWhiteSpace(template.TemplateLanguageLocale))
                                  orderby template.TemplateLanguageLocale descending
                                  select template).FirstOrDefault();

            if (templateConfig != null)
                return (templateConfig.TemplateEmailTitle, BuildEmailContent(templateName, templateConfig.TemplateFilePath, values, languageLocale));
            else
                throw new Exception($"Template name {templateName} with language locale as '{languageLocale ?? ""}' doesn't exist.");
        }

        /// <summary>
        /// Build email content based on template and values
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="templateFilePath"></param>
        /// <param name="values"></param>
        /// <param name="languageLocale"></param>
        /// <returns></returns>
        private string BuildEmailContent(string templateName, string templateFilePath, (string key, string value)[] values, string languageLocale)
        {
            var cachedTemmplatedName = languageLocale != null ? $"{templateName}.{languageLocale.Trim()}" : templateName;

            if (!CheckIsTemplateCached(cachedTemmplatedName))
                CachedTemplates.Add(cachedTemmplatedName, ReadTemplateFile(templateFilePath));

            string templateContent = CachedTemplates[cachedTemmplatedName];
            foreach (var kv in values)
            {
                // Replace placeholders with actual value.
                templateContent = templateContent.Replace($"{{{kv.key}}}", kv.value);
            }
            return templateContent;
        }

        /// <summary>
        /// Read file content
        /// </summary>
        /// <param name="templateFilePath"></param>
        /// <returns></returns>
        private string ReadTemplateFile(string templateFilePath)
        {
            IFileInfo fileInfo = fileProvider.GetFileInfo(templateFilePath);

            if (!fileInfo.Exists)
                throw new FileNotFoundException($"Template file located at \"{templateFilePath}\" was not found");

            using (var fs = fileInfo.CreateReadStream())
            {
                using (var sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
