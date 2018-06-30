namespace Kontext.Configuration
{
    public class EmailTemplateConfig : IEmailTemplateConfig
    {
        public string TemplateName { get; set; }
        public string TemplateFilePath { get; set; }
        public string TemplateEmailTitle { get; set; }
        public string TemplateLanguageLocale { get; set; }
    }
}
