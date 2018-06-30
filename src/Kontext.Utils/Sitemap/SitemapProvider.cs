using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kontext.Utils.Sitemap
{
    /// <summary>
    /// Sitemap provider
    /// </summary>
    public class SitemapProvider
    {
        ICollection<SitemapUrl> urls;

        public SitemapProvider()
        {
            Urls = new Collection<SitemapUrl>();
        }

        public ICollection<SitemapUrl> Urls { get => urls; set => urls = value; }

        /// <summary>
        /// Add url
        /// </summary>
        /// <param name="url"></param>
        public void AddUrl(SitemapUrl url) => this.Urls.Add(url);

        public void AddUrl(string url, double? priority = null, DateTime? lastModified = null, ChangeFrequency changeFrequency = ChangeFrequency.Weekly)
        {
            this.Urls.Add(new SitemapUrl { Url = url, ChangeFrequency = changeFrequency, Priority = priority, LastModified = lastModified });
        }

        public void AddUrl(string url, DateTime lastModified)
        {
            this.Urls.Add(new SitemapUrl { Url = url, LastModified = lastModified });
        }
    }
}
