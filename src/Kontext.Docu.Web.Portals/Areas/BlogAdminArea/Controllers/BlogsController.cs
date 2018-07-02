using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Kontext.Docu.Web.Portals.Areas.BlogAdminArea.Controllers
{
    [Area("BlogAdminArea")]
    [Authorize]
    public class BlogsController : Controller
    {
        private readonly ContextBlogDbContext _context;

        public BlogsController(ContextBlogDbContext context)
        {
            _context = context;
        }

        // GET: BlogAdminArea/Blogs
        [Authorize(BlogApplicationAuthorizationPolicies.ViewBlogPolicy)]
        public async Task<IActionResult> Index()
        {
            var contextBlogDbContext = _context.Blogs.Include(b => b.Language);
            return View(await contextBlogDbContext.ToListAsync());
        }

        // GET: BlogAdminArea/Blogs/Details/5
        [Authorize(BlogApplicationAuthorizationPolicies.ViewBlogPolicy)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.Language)
                .SingleOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        [Authorize(BlogApplicationAuthorizationPolicies.AddBlogPolicy)]
        // GET: BlogAdminArea/Blogs/Create
        public IActionResult Create()
        {
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode");
            return View();
        }

        // POST: BlogAdminArea/Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.AddBlogPolicy)]
        public async Task<IActionResult> Create([Bind("BlogId,UserId,BlogGroupId,Title,SubTitle,UniqueName,IsActive,LanguageCode,SkinCssFile,SecondaryCss,PostCount,CommentCount,FileCount,PingTrackCount,News,TrackingCode,Tag,DateCreated,DateModified")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blog.LanguageCode);
            return View(blog);
        }

        // GET: BlogAdminArea/Blogs/Edit/5
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogPolicy)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.SingleOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blog.LanguageCode);
            return View(blog);
        }

        // POST: BlogAdminArea/Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogPolicy)]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,UserId,BlogGroupId,Title,SubTitle,UniqueName,IsActive,LanguageCode,SkinCssFile,SecondaryCss,PostCount,CommentCount,FileCount,PingTrackCount,News,TrackingCode,Tag,DateCreated,DateModified")] Blog blog)
        {
            if (id != blog.BlogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.BlogId))
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
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blog.LanguageCode);
            return View(blog);
        }

        // GET: BlogAdminArea/Blogs/Delete/5
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogPolicy)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.Language)
                .SingleOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: BlogAdminArea/Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogPolicy)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.SingleOrDefaultAsync(m => m.BlogId == id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }
    }
}
