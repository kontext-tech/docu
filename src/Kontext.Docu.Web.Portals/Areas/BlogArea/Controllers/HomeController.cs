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
    public class HomeController : Controller
    {
        private readonly IContextBlogUnitOfWork unitOfWork;
        private readonly IConfigService configService;

        public HomeController(IContextBlogUnitOfWork unitOfWork, IConfigService configService)
        {
            this.unitOfWork = unitOfWork;
            this.configService = configService;
        }

        [Route("/Blog")]
        [ActionName("Index2")]
        public IActionResult Index2()
        {
            return View("Index");
        }


        [Route("/docs")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Show blog details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("/Blog/{name}")]
        [ActionName("Detail2")]
        public async Task<IActionResult> Detail2(string name)
        {
            return await Detail2(name, 1);
        }

        /// <summary>
        /// Show blog details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("/docs/{name}")]
        [ActionName("Detail")]
        public async Task<IActionResult> Detail(string name)
        {
            return await Detail(name, 1);
        }

        [Route("/docs/{name}/All/{page}")]
        [ActionName("Detail")]
        public async Task<IActionResult> Detail(string name, int page = 1)
        {
            var blog = await unitOfWork.BlogRepository.QueryOneAsync(b => b.UniqueName == name, nameof(Blog.Categories));
            var posts = await GetBlogPostListWithCategoryAndPosts(blog.BlogId, page);
            var pagedVM = new PagedViewModel<BlogViewModel, BlogPost>(AutoMapper.Mapper.Map<BlogViewModel>(blog), posts, posts.TotalItemCount, posts.PageCount, posts.PageSize, posts.PageNumber);
            return View("_BlogDetail", pagedVM);
        }

        [Route("/Blog/{name}/All/{page}")]
        [ActionName("Detail2")]
        public async Task<IActionResult> Detail2(string name, int page = 1)
        {
            var blog = await unitOfWork.BlogRepository.QueryOneAsync(b => b.UniqueName == name, nameof(Blog.Categories));
            var posts = await GetBlogPostListWithCategoryAndPosts(blog.BlogId, page);
            var pagedVM = new PagedViewModel<BlogViewModel, BlogPost>(AutoMapper.Mapper.Map<BlogViewModel>(blog), posts, posts.TotalItemCount, posts.PageCount, posts.PageSize, posts.PageNumber);
            return View("_BlogDetail", pagedVM);
        }

        private async Task<IPagedList<BlogPost>> GetBlogPostListWithCategoryAndPosts(int blogId, int page = 1)
        {
            var query = from post in unitOfWork.BlogPostRepository.Entities
                        .Include(b => b.Tags)
                        .ThenInclude(e => e.Tag)
                        .Include(b => b.BlogCategories)
                        where post.BlogId == blogId && !post.IsDeleted && post.DatePublished.HasValue
                        orderby post.DateModified descending
                        select post;
            return await query.ToPagedListAsync(page, configService.BlogConfig.BlogPostCountPerPage);

        }
    }
}