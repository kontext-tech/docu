using Kontext.Data;
using Kontext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kontext.Docu.Web.Portals.Areas.BlogArea.Controllers
{
    [Produces("application/rss+xml")]
    [Route("/Syndication")]
    [Area("BlogArea")]
    public class RssController : Controller
    {
        private readonly IConfigService configService;
        private readonly IContextBlogUnitOfWork unitOfWork;

        public RssController(IConfigService configService, IContextBlogUnitOfWork unitOfWork)
        {
            this.configService = configService;
            this.unitOfWork = unitOfWork;
        }

        [Route("RSS.xml")]
        public async Task<IActionResult> Rss20AllAsync()
        {
            return await Rss20Async(-1);
        }

        [Route("RSS/{blogId}.xml")]
        public async Task<IActionResult> Rss20Async(int blogId)
        {
            var query = from post in unitOfWork.BlogPostRepository.Entities.Include(e => e.Blog).Include(e => e.BlogCategories)
                        where (blogId == -1 || (blogId == post.BlogId))
                        && post.IsDeleted == false && post.DatePublished.HasValue
                        orderby post.DatePublished descending
                        select post;

            var posts = await query.Take(configService.BlogConfig.RssItemsCount).ToListAsync();
            var allCategories = unitOfWork.BlogCategoryRepository.GetAll();

            var title = "";
            var desc = "";
            var link = "";
            if (blogId == -1)
            {
                title = configService.SiteConfig.SiteName;
                desc = configService.SiteConfig.SiteDescription;
                link = Url.HomePageLink(Request.Scheme);
            }
            else
            {
                var post = posts.FirstOrDefault();
                if (post != null)
                {
                    title = post.Blog.Title;
                    desc = post.Blog.SubTitle?.ToString();
                    link = Url.BlogLink(post.Blog.UniqueName, Request.Scheme);
                }
            }

            using (MemoryStream memStream = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    Async = true
                };
                using (XmlWriter xmlWriter = XmlWriter.Create(memStream, xmlWriterSettings))
                {
                    var writer = new RssFeedWriter(xmlWriter);
                    //
                    // Add Title
                    await writer.WriteTitle(title);
                    //
                    // Add Description
                    await writer.WriteDescription(desc);
                    //
                    // Add Link
                    await writer.Write(new SyndicationLink(new Uri(link)));

                    //
                    // Add publish date
                    await writer.WritePubDate(DateTimeOffset.UtcNow);

                    // Write each post
                    foreach (var post in posts)
                    {
                        // Create item
                        var item = new SyndicationItem()
                        {
                            Title = post.Title,
                            Description = post.Text,
                            Id = Url.BlogPostLink(post.Blog.UniqueName, post.UniqueName, post.DatePublished.Value, Request.Scheme),
                            Published = DateTimeOffset.UtcNow
                        };

                        item.AddLink(new SyndicationLink(new Uri(Url.BlogPostLink(post.Blog.UniqueName, post.UniqueName, post.DatePublished.Value, Request.Scheme))));

                        // Category
                        foreach (var cate in post.BlogCategories)
                        {
                            var cateWithInfo = allCategories.FirstOrDefault(c => cate.BlogCategoryId == c.BlogCategoryId);
                            if (cateWithInfo != null)
                            {
                                item.AddCategory(new SyndicationCategory(cateWithInfo.Title));
                            }
                        }

                        // Contributor 
                        item.AddContributor(new SyndicationPerson(post.Author, post.Email));

                        await writer.Write(item);
                    }
                    xmlWriter.Flush();
                }
                return Content(Encoding.UTF8.GetString(memStream.ToArray()), "application/rss+xml", Encoding.UTF8);
            }

        }

    }
}