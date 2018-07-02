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
    [ViewComponent(Name = "BlogPostCommentList")]
    public class BlogPostCommentListViewComponent : ViewComponent
    {
        private readonly IContextBlogUnitOfWork unitWork;
        private readonly IConfigService configService;
        private readonly ICacheManager cacheManager;
        private readonly ILogger<BlogPostCommentListViewComponent> logger;

        public BlogPostCommentListViewComponent(IContextBlogUnitOfWork unitWork, IConfigService configService, ICacheManager cacheManager, ILogger<BlogPostCommentListViewComponent> logger)
        {
            this.unitWork = unitWork;
            this.configService = configService;
            this.cacheManager = cacheManager;
            this.logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? blogPostId, int? maxCommentCount, IEnumerable<BlogPostComment> comments, string viewName = "Default", string type = "Default", bool includingDeleted = false)
        {
            if (comments == null)
            {
                var key = $"{nameof(BlogPostCommentListViewComponent)}_{type}_{includingDeleted}";

                // comments for home page using cache
                if (blogPostId == null)
                {
                    if (!cacheManager.TryGetValue(key, out IEnumerable<BlogPostComment> topComments))
                    {
                        logger.LogCritical("Caching blog comments list for home page.");

                        IQueryable<BlogPostComment> query = from c in unitWork.BlogPostCommentRepository.Entities.Include(e => e.ReplyToBlogPostComment).Include(e => e.Blog).Include(e => e.BlogPost)
                                                            where (includingDeleted || !c.IsDeleted) && (blogPostId == null || c.BlogPostId.Value == blogPostId.Value)
                                                            orderby c.DateCreated descending
                                                            select c;

                        if (maxCommentCount.HasValue)
                            query = query.Take(maxCommentCount.Value);

                        topComments = await query.ToListAsync();

                        // Save data in cache.
                        cacheManager.SetAbsoluteExpiration(key, topComments, TimeSpan.FromDays(1));
                    }

                    comments = topComments;
                }
                else
                {
                    comments = from c in unitWork.BlogPostCommentRepository.Entities.Include(e => e.ReplyToBlogPostComment).Include(e => e.Blog).Include(e => e.BlogPost)
                               where (includingDeleted || !c.IsDeleted) && (blogPostId == null || c.BlogPostId.Value == blogPostId.Value)
                               orderby c.DateCreated descending
                               select c;
                }
            }

            if (maxCommentCount.HasValue)
                return View(viewName, comments.Take(maxCommentCount.Value).ToList());
            else if (type != "Default")
                return View(viewName, comments.Take(configService.BlogConfig.HomePageBlogLatestCommentCount).ToList());
            else
                return View(viewName, comments.ToList());
        }
    }
}
