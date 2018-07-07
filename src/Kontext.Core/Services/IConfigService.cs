using Kontext.Configuration;

namespace Kontext.Services
{
    public interface IConfigService
    {
        ISiteConfig SiteConfig { get; }

        IBlogConfig BlogConfig { get; }

        ISecurityConfig SecurityConfig { get; }

        IEmailConfig EmailConfig { get; }

        IEmailTemplateConfig[] EmailTemplates { get; }

        IDatabaseConfig DatabaseConfig { get; }
    }
}
