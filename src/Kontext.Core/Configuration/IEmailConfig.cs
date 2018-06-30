namespace Kontext.Configuration
{
    public interface IEmailConfig
    {
        string FromEmail { get; set; }
        string FromEmailDisplayName { get; set; }
        bool EnableSSl { get; set; }
        string Host { get; set; }
        string PasswordConfigurationName { get; set; }
        int Port { get; set; }
        string UserName { get; set; }
    }
}