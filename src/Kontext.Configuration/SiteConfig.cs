namespace Kontext.Configuration
{
    public class SiteConfig : ISiteConfig
    {

        public string SiteName { get; set; }
        public string SiteDomain { get; set; }
        public string SiteNewDomain { get; set; }
        public bool ShowNewDomainReminder { get; set; }
        public string AppVersion { get; set; }
        public string SiteAuthor { get; set; }
        public string SiteKeyWords { get; set; }
        public string SiteDescription { get; set; }
        public string ImageFolderPath { get; set; }
        public string DefaultApplicationId { get; set; }
    }
}