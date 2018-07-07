using Kontext.Configuration;
using Microsoft.Extensions.Options;

namespace Kontext.Services
{
    public class ConfigurationService : IConfigService
    {
        private readonly IOptions<SiteConfig> siteConfig;
        private readonly IOptions<EmailConfig> emailConfig;
        private readonly IOptions<SecurityConfig> securityConfig;
        private readonly IOptions<BlogConfig> blogConfig;
        private readonly IOptions<EmailTemplatesConfig> emailTemplatesConfig;
        private readonly IOptions<DatabaseConfig> databaseConfig;

        public ConfigurationService(IOptions<SiteConfig> siteConfig, IOptions<EmailConfig> emailConfig, IOptions<SecurityConfig> securityConfig, IOptions<BlogConfig> blogConfig, IOptions<EmailTemplatesConfig> emailTemplatesConfig, IOptions<DatabaseConfig> databaseConfig)
        {
            this.siteConfig = siteConfig;
            this.emailConfig = emailConfig;
            this.securityConfig = securityConfig;
            this.blogConfig = blogConfig;
            this.emailTemplatesConfig = emailTemplatesConfig;
            this.databaseConfig = databaseConfig;
        }

        public ISiteConfig SiteConfig => siteConfig.Value;

        public IBlogConfig BlogConfig => blogConfig.Value;

        public ISecurityConfig SecurityConfig => securityConfig.Value;

        public IEmailConfig EmailConfig => emailConfig.Value;

        public IEmailTemplateConfig[] EmailTemplates => emailTemplatesConfig.Value.EmailTemplates;

        public IDatabaseConfig DatabaseConfig => databaseConfig.Value;
    }
}
