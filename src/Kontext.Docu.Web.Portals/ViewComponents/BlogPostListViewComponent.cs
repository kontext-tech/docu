using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontext.Docu.Web.Portals.ViewComponents
{
    [ViewComponent(Name = "BlogPostList")]
    public class BlogPostListViewComponent : ViewComponent
    {
        private readonly IContextBlogUnitOfWork unitWork;
        private readonly IConfigService configService;
        private readonly ICacheManager cacheManager;
        private readonly ILogger<BlogPostListViewComponent> logger;

        public BlogPostListViewComponent(IContextBlogUnitOfWork unitWork, IConfigService configService, ICacheManager cacheManager, ILogger<BlogPostListViewComponent> logger)
        {
            this.unitWork = unitWork;
            this.configService = configService;
            this.cacheManager = cacheManager;
            this.logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default", string type = "Latest", bool includingNonActive = false)
        {
            var key = $"{nameof(BlogPostListViewComponent)}_{includingNonActive}_{type}";

            if (!cacheManager.TryGetValue(key, out IEnumerable<BlogPost> blogPosts))
            {
                logger.LogCritical("Caching blog post list");
                var query = from p in unitWork.BlogPostRepository.Entities.Include(p => p.Blog)
                            .Include(e => e.Tags)
                            .ThenInclude(e => e.Tag)
                            where p.IsDeleted == false && p.DatePublished.HasValue
                            select p;
                if (type == "Latest")
                {
                    query = query.OrderByDescending(p => p.DatePublished).Take(configService.BlogConfig.HomePageBlogLatestPostCount);
                }
                else if (type == "Comments")
                {
                    query = query.OrderByDescending(p => p.CommentCount).Take(configService.BlogConfig.HomePageBlogLatestPostCount);
                }
                else
                {
                    query = query.OrderByDescending(p => p.ViewCount).Take(configService.BlogConfig.HomePageBlogLatestPostCount);
                }

                blogPosts = await query.ToListAsync();
                // Save data in cache.
                cacheManager.SetAbsoluteExpiration(key, blogPosts, TimeSpan.FromHours(6));
            }

            return View(viewName, blogPosts);
        }

    }
}
