using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Kontext.Docu.Web.Portals.Areas.BlogAdminArea.Controllers
{
    [Area("BlogAdminArea")]
    [Authorize]
    public class BlogPostCommentsController : Controller
    {
        private readonly ContextBlogDbContext _context;

        public BlogPostCommentsController(ContextBlogDbContext context)
        {
            _context = context;
        }

        // GET: BlogAdminArea/BlogPostComments
        [Authorize(BlogApplicationAuthorizationPolicies.ViewBlogCommentPolicy)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var entities = _context.BlogPostComments.Include(b => b.Blog).Include(b => b.BlogPost).Include(b => b.Language).Include(b => b.ReplyToBlogPostComment).OrderByDescending(b => b.DateModified);
            var pagedList = await entities.ToPagedListAsync(page, pageSize);
            var pagedVM = new PagedViewModel<object, BlogPostComment>(null, pagedList, pagedList.TotalItemCount, pagedList.PageCount, pageSize, page);

            return View(pagedVM);
        }

        // GET: BlogAdminArea/BlogPostComments/Details/5
        [Authorize(BlogApplicationAuthorizationPolicies.ViewBlogCommentPolicy)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostComment = await _context.BlogPostComments
                .Include(b => b.Blog)
                .Include(b => b.BlogPost)
                .Include(b => b.Language)
                .Include(b => b.ReplyToBlogPostComment)
                .SingleOrDefaultAsync(m => m.BlogPostCommentId == id);
            if (blogPostComment == null)
            {
                return NotFound();
            }

            return View(blogPostComment);
        }

        // GET: BlogAdminArea/BlogPostComments/Edit/5
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCommentPolicy)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostComment = await _context.BlogPostComments.SingleOrDefaultAsync(m => m.BlogPostCommentId == id);
            if (blogPostComment == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title", blogPostComment.BlogId);
            ViewData["BlogPostId"] = new SelectList(_context.BlogPosts.Where(e => e.BlogPostId == blogPostComment.BlogPostId), "BlogPostId", "Title", blogPostComment.BlogPostId);
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blogPostComment.LanguageCode);
            ViewData["ReplyToBlogPostCommentId"] = new SelectList(_context.BlogPostComments.Where(e => e.BlogPostId == blogPostComment.BlogPostId), "BlogPostCommentId", "Title", blogPostComment.ReplyToBlogPostCommentId);
            return View(blogPostComment);
        }

        // POST: BlogAdminArea/BlogPostComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCommentPolicy)]
        public async Task<IActionResult> Edit(int id, [Bind("BlogPostCommentId,Approved,Title,Author,IsBlogUser,UserId,LanguageCode,Email,Text,IpAddress,UserAgent,Tag,IsDeleted,DateDeleted,DateCreated,DateModified,BlogId,BlogPostId,ReplyToBlogPostCommentId")] BlogPostComment blogPostComment)
        {
            if (id != blogPostComment.BlogPostCommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogPostComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostCommentExists(blogPostComment.BlogPostCommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title", blogPostComment.BlogId);
            ViewData["BlogPostId"] = new SelectList(_context.BlogPosts.Where(e => e.BlogPostId == blogPostComment.BlogPostId), "BlogPostId", "Title", blogPostComment.BlogPostId);
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blogPostComment.LanguageCode);
            ViewData["ReplyToBlogPostCommentId"] = new SelectList(_context.BlogPostComments, "BlogPostCommentId", "Title", blogPostComment.ReplyToBlogPostCommentId);
            return View(blogPostComment);
        }

        // GET: BlogAdminArea/BlogPostComments/Delete/5
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCommentPolicy)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostComment = await _context.BlogPostComments
                .Include(b => b.Blog)
                .Include(b => b.BlogPost)
                .Include(b => b.Language)
                .Include(b => b.ReplyToBlogPostComment)
                .SingleOrDefaultAsync(m => m.BlogPostCommentId == id);
            if (blogPostComment == null)
            {
                return NotFound();
            }

            return View(blogPostComment);
        }

        // POST: BlogAdminArea/BlogPostComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCommentPolicy)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPostComment = await _context.BlogPostComments.SingleOrDefaultAsync(m => m.BlogPostCommentId == id);
            _context.BlogPostComments.Remove(blogPostComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostCommentExists(int id)
        {
            return _context.BlogPostComments.Any(e => e.BlogPostCommentId == id);
        }
    }
}
