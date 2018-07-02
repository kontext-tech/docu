using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Kontext.Docu.Web.Portals.Areas.BlogArea.Controllers
{
    [Area("BlogArea")]
    [Route("/tag")]
    public class TagController : Controller
    {

        private readonly IContextBlogUnitOfWork unitOfWork;
        private readonly IConfigService configService;

        public TagController(IContextBlogUnitOfWork unitOfWork, IConfigService configService)
        {
            this.unitOfWork = unitOfWork;
            this.configService = configService;
        }

        [Route("{tagName}")]
        public async Task<IActionResult> IndexAsync(string tagName)
        {
            return await IndexPageAsync(tagName, 1);
        }

        [Route("{tagName}.html")]
        public async Task<IActionResult> Index2Async(string tagName)
        {
            return await IndexPageAsync(tagName, 1);
        }


        [Route("{tagName}/{page}.html")]
        public async Task<IActionResult> IndexPageAsync(string tagName, int page)
        {
            var tag = await unitOfWork.TagRepository.QueryOneAsync(t => t.TagName == tagName, nameof(Tag.BlogPosts));
            var posts = await GetBlogPostList(tag.TagId, page);
            var pagedVM = new PagedViewModel<Tag, BlogPost>(tag, posts, posts.TotalItemCount, posts.PageCount, posts.PageSize, posts.PageNumber);
            return View("_TagIndex", pagedVM);
        }

        private async Task<IPagedList<BlogPost>> GetBlogPostList(int tagId, int page = 1)
        {
            var query = from tag in unitOfWork.BlogPostTagRepository.Entities
                        join post in unitOfWork.BlogPostRepository.Entities.Include(p => p.Blog)
                        on tag.BlogPostId equals post.BlogPostId
                        where tag.TagId == tagId && !post.IsDeleted && post.DatePublished.HasValue
                        orderby tag.BlogPost.DateModified descending
                        select post;
            return await query.ToPagedListAsync(page, configService.BlogConfig.BlogPostCountPerPage);
        }
    }
}