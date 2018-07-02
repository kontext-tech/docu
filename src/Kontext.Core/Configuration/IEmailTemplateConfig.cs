namespace Kontext.Configuration
{
    public interface IEmailTemplateConfig
    {
        string TemplateName { get; set; }
        string TemplateFilePath { get; set; }
        string TemplateEmailTitle { get; set; }
        string TemplateLanguageLocale { get; set; }
    }
}
