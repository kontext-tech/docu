using System;

namespace Kontext.Utils.Sitemap
{
    public class SitemapUrl
    {
        public ChangeFrequency? ChangeFrequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }
    }
}
