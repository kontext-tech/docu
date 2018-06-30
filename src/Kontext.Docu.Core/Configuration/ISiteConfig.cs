namespace Kontext.Configuration
{
    /// <summary>
    /// Context project core configurations.
    /// </summary>
    public interface ISiteConfig
    {
        string SiteName { get; set; }
        string SiteDomain { get; set; }
        string SiteNewDomain { get; set; }
        bool ShowNewDomainReminder { get; set; }
        string AppVersion { get; set; }
        string SiteAuthor { get; set; }
        string SiteKeyWords { get; set; }
        string SiteDescription { get; set; }
        string ImageFolderPath { get; set; }
        string DefaultApplicationId { get; set; }
    }
}
