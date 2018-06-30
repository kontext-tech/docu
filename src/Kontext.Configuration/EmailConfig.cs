namespace Kontext.Configuration
{
    public class EmailConfig : IEmailConfig
    {
        public string FromEmail { get; set; }
        public string FromEmailDisplayName { get; set; }
        public bool EnableSSl { get; set; }
        public string Host { get; set; }
        public string PasswordConfigurationName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
    }
}
