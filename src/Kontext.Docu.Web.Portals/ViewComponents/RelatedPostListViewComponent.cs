using Kontext.Data;
using Kontext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontext.Docu.Web.Portals.ViewComponents
{
    [ViewComponent(Name = "RelatedPostList")]
    public class RelatedPostListViewComponent : ViewComponent
    {
        private readonly IContextBlogUnitOfWork unitWork;
        private readonly IConfigService configService;
        private readonly ILogger<RelatedPostListViewComponent> logger;

        public RelatedPostListViewComponent(IContextBlogUnitOfWork unitWork, IConfigService configService, ILogger<RelatedPostListViewComponent> logger)
        {
            this.unitWork = unitWork;
            this.configService = configService;
            this.logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(int postId, string viewName = "Default")
        {
            var query = from cate in unitWork.BlogPostCategoryRespository.Entities.Include(c => c.BlogCategory)
                        join cate2 in unitWork.BlogPostCategoryRespository.Entities.Include(c => c.BlogPost).ThenInclude(b => b.Tags).ThenInclude(t => t.Tag)
                        on cate.BlogCategoryId equals cate2.BlogCategoryId
                        where cate.BlogPostId == postId && cate2.BlogPostId != postId && !cate2.BlogPost.IsDeleted && cate2.BlogPost.DatePublished.HasValue
                        orderby cate2.BlogPost.DateModified descending
                        select cate2.BlogPost
                        ;

            var posts = await query.Take(6).ToListAsync();

            return View(viewName, posts);
        }

    }
}
