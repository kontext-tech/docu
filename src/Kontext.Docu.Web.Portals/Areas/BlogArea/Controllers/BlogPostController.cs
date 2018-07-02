using Kontext.Data;
using Kontext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;


namespace Kontext.Docu.Web.Portals.Areas.BlogArea.Controllers
{
    [Area("BlogArea")]
    public class BlogPostController : Controller
    {
        private readonly IContextBlogUnitOfWork unitOfWork;
        private readonly IConfigService configService;

        public BlogPostController(IContextBlogUnitOfWork unitOfWork, IConfigService configService)
        {
            this.unitOfWork = unitOfWork;
            this.configService = configService;
        }

        [Route("/docs/{blogName}/p/{postName}")]
        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync(string blogName, string postName)
        {
            var query = from p in unitOfWork.BlogPostRepository.Entities
                        .Include(e => e.BlogCategories)
                        .ThenInclude(e => e.BlogCategory)
                        .Include(e => e.Blog)
                        .Include(e => e.Tags)
                        .ThenInclude(e => e.Tag)
                        where p.UniqueName == postName
                        select p;
            var post = await query.FirstOrDefaultAsync();
            if (post.Tag != null && post.Tag == Constants.LiteLogType)
                return View("Lite", post);
            else
                return View("Index", post);
        }

        [Route("/Blog/{blogName}/Archive/{year}/{month}/{day}/{postName}.html")]
        [ActionName("Index2")]
        public async Task<IActionResult> IndexAsync(string blogName, string categoryName, int year, int month, int day, string postName)
        {
            return await IndexAsync(blogName, postName);
        }

        [Route("/docs/post/{blogPostId}/ViewCount")]
        [HttpGet]
        [Produces("application/json")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateViewCountAsync(int blogPostId)
        {
            var post = await unitOfWork.BlogPostRepository.FindOneAsync(p => p.BlogPostId == blogPostId);
            int viewCount = 0;
            if (post != null)
            {
                post.ViewCount += 1;
                viewCount = post.ViewCount.Value;
                unitOfWork.BlogPostRepository.Update(post);
                unitOfWork.SaveChanges();
            }
            return Json(new { ViewCount = viewCount });
        }
    }
}