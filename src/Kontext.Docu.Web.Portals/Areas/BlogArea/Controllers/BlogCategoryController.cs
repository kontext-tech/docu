using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using X.PagedList;
using System.Linq;

namespace Kontext.Docu.Web.Portals.Areas.BlogArea.Controllers
{
    [Area("BlogArea")]
    public class BlogCategoryController : Controller
    {

        private readonly IContextBlogUnitOfWork unitOfWork;
        private readonly IConfigService configService;

        public BlogCategoryController(IContextBlogUnitOfWork unitOfWork, IConfigService configService)
        {
            this.unitOfWork = unitOfWork;
            this.configService = configService;
        }

        [Route("/docs/{blogName}/c/{categoryName}/{page?}")]
        [ActionName("Index")]
        public async Task<IActionResult> Index(string blogName, string categoryName, int? page = 1)
        {
            return await Index2(blogName, categoryName, page);
        }

        [Route("/Blog/{blogName}/{categoryName}/{page?}")]
        [ActionName("Index2")]
        public async Task<IActionResult> Index2(string blogName, string categoryName, int? page = 1)
        {
            var category = await unitOfWork.BlogCategoryRepository.QueryOneAsync(b => b.UniqueName == categoryName, nameof(BlogCategory.Blog));
            var posts = await GetBlogPostListWithCategoryAndPosts(category.BlogId, category.BlogCategoryId, page.Value);
            var pagedVM = new PagedViewModel<BlogCategory, BlogPost>(category, posts, posts.TotalItemCount, posts.PageCount, posts.PageSize, posts.PageNumber);
            return View("_BlogCategoryIndex", pagedVM);
        }

        private async Task<IPagedList<BlogPost>> GetBlogPostListWithCategoryAndPosts(int blogId, int categoryId, int page = 1)
        {
            var q = from post in unitOfWork.BlogPostRepository.Entities.Include(e => e.Tags).ThenInclude(e => e.Tag)
                    join cate in unitOfWork.BlogPostCategoryRespository.Entities on post.BlogPostId equals cate.BlogPostId
                    where cate.BlogCategoryId == categoryId && post.BlogId == blogId && !post.IsDeleted && post.DatePublished.HasValue
                    orderby cate.BlogPost.DateModified descending
                    select post;
            //var query = from category in unitOfWork.BlogPostCategoryRespository.Entities.Include(b => b.BlogPost)
            //            where category.BlogCategoryId == categoryId && !category.BlogPost.IsDeleted && category.BlogPost.DatePublished.HasValue
            //            orderby category.BlogPost.DateModified descending
            //            select category.BlogPost;
            return await q.ToPagedListAsync(page, configService.BlogConfig.BlogPostCountPerPage);
        }
    }
}