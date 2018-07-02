using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kontext.Data;
using Kontext.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Kontext.Security;
using X.PagedList;
using Kontext.Data.Models.ViewModels;

namespace Kontext.Docu.Web.Portals.Areas.BlogAdminArea.Controllers
{
    [Area("BlogAdminArea")]
    [Authorize]
    public class BlogPostsController : Controller
    {
        private readonly ContextBlogDbContext _context;

        public BlogPostsController(ContextBlogDbContext context)
        {
            _context = context;
        }

        // GET: BlogAdminArea/BlogPosts
        [Authorize(BlogApplicationAuthorizationPolicies.ViewBlogPostPolicy)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var entities = _context.BlogPosts.Include(b => b.Blog).Include(b => b.Language).OrderByDescending(b => b.DatePublished);
            var pagedList = await entities.ToPagedListAsync(page, pageSize);
            var pagedVM = new PagedViewModel<object, BlogPost>(null, pagedList, pagedList.TotalItemCount, pagedList.PageCount, pageSize, page);

            return View(pagedVM);
        }

        // GET: BlogAdminArea/BlogPosts/Details/5
        [Authorize(BlogApplicationAuthorizationPolicies.ViewBlogPostPolicy)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .Include(b => b.Language)
                .SingleOrDefaultAsync(m => m.BlogPostId == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogAdminArea/BlogPosts/Create
        [Authorize(BlogApplicationAuthorizationPolicies.AddBlogPostPolicy)]
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title");
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode");
            return View();
        }

        // POST: BlogAdminArea/BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.AddBlogPostPolicy)]
        public async Task<IActionResult> Create([Bind("BlogPostId,Title,Author,Email,LanguageCode,Description,KeyWords,Text,ViewCount,CommentCount,UniqueName,IpAddress,Tag,IsDeleted,DateDeleted,DateCreated,DateModified,DatePublished,BlogId")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title", blogPost.BlogId);
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blogPost.LanguageCode);
            return View(blogPost);
        }

        // GET: BlogAdminArea/BlogPosts/Edit/5
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogPostPolicy)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.SingleOrDefaultAsync(m => m.BlogPostId == id);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title", blogPost.BlogId);
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blogPost.LanguageCode);
            return View(blogPost);
        }

        // POST: BlogAdminArea/BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogPostPolicy)]
        public async Task<IActionResult> Edit(int id, [Bind("BlogPostId,Title,Author,Email,LanguageCode,Description,KeyWords,Text,ViewCount,CommentCount,UniqueName,IpAddress,Tag,IsDeleted,DateDeleted,DateCreated,DateModified,DatePublished,BlogId")] BlogPost blogPost)
        {
            if (id != blogPost.BlogPostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.BlogPostId))
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title", blogPost.BlogId);
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blogPost.LanguageCode);
            return View(blogPost);
        }

        // GET: BlogAdminArea/BlogPosts/Delete/5
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogPostPolicy)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .Include(b => b.Language)
                .SingleOrDefaultAsync(m => m.BlogPostId == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogAdminArea/BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogPostPolicy)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.SingleOrDefaultAsync(m => m.BlogPostId == id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.BlogPostId == id);
        }
    }
}
