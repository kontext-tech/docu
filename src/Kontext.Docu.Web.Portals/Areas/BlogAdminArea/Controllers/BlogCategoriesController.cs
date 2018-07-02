using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Kontext.Docu.Web.Portals.Areas.BlogAdminArea.Controllers
{
    [Area("BlogAdminArea")]
    [Authorize]
    public class BlogCategoriesController : Controller
    {
        private readonly ContextBlogDbContext _context;

        public BlogCategoriesController(ContextBlogDbContext context)
        {
            _context = context;
        }

        // GET: BlogAdminArea/BlogCategories
        [Authorize(BlogApplicationAuthorizationPolicies.ViewBlogCategoryPolicy)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var entities = _context.BlogCategories.Include(b => b.Blog).Include(b => b.Language).OrderByDescending(e => e.DateModified);
            var pagedList = await entities.ToPagedListAsync(page, pageSize);
            var pagedVM = new PagedViewModel<object, BlogCategory>(null, pagedList, pagedList.TotalItemCount, pagedList.PageCount, pageSize, page);

            return View(pagedVM);
        }

        // GET: BlogAdminArea/BlogCategories/Details/5
        [Authorize(BlogApplicationAuthorizationPolicies.ViewBlogCategoryPolicy)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogCategory = await _context.BlogCategories
                .Include(b => b.Blog)
                .Include(b => b.Language)
                .SingleOrDefaultAsync(m => m.BlogCategoryId == id);
            if (blogCategory == null)
            {
                return NotFound();
            }

            return View(blogCategory);
        }

        // GET: BlogAdminArea/BlogCategories/Create
        [Authorize(BlogApplicationAuthorizationPolicies.AddBlogCategoryPolicy)]
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title");
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode");
            return View();
        }

        // POST: BlogAdminArea/BlogCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.AddBlogCategoryPolicy)]
        public async Task<IActionResult> Create([Bind("BlogCategoryId,UniqueName,Title,Active,Description,LanguageCode,Tag,DateCreated,DateModified,BlogId")] BlogCategory blogCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title", blogCategory.BlogId);
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blogCategory.LanguageCode);
            return View(blogCategory);
        }

        // GET: BlogAdminArea/BlogCategories/Edit/5
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCategoryPolicy)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogCategory = await _context.BlogCategories.SingleOrDefaultAsync(m => m.BlogCategoryId == id);
            if (blogCategory == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title", blogCategory.BlogId);
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blogCategory.LanguageCode);
            return View(blogCategory);
        }

        // POST: BlogAdminArea/BlogCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCategoryPolicy)]
        public async Task<IActionResult> Edit(int id, [Bind("BlogCategoryId,UniqueName,Title,Active,Description,LanguageCode,Tag,DateCreated,DateModified,BlogId")] BlogCategory blogCategory)
        {
            if (id != blogCategory.BlogCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogCategoryExists(blogCategory.BlogCategoryId))
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "Title", blogCategory.BlogId);
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", blogCategory.LanguageCode);
            return View(blogCategory);
        }

        // GET: BlogAdminArea/BlogCategories/Delete/5
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCategoryPolicy)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogCategory = await _context.BlogCategories
                .Include(b => b.Blog)
                .Include(b => b.Language)
                .SingleOrDefaultAsync(m => m.BlogCategoryId == id);
            if (blogCategory == null)
            {
                return NotFound();
            }

            return View(blogCategory);
        }

        // POST: BlogAdminArea/BlogCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCategoryPolicy)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogCategory = await _context.BlogCategories.SingleOrDefaultAsync(m => m.BlogCategoryId == id);
            _context.BlogCategories.Remove(blogCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogCategoryExists(int id)
        {
            return _context.BlogCategories.Any(e => e.BlogCategoryId == id);
        }

        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCategoryPolicy)]
        [HttpGet]
        public async Task<IActionResult> MergeCategory(int id)
        {
            var fromCategory = _context.BlogCategories.Where(e => e.BlogCategoryId == id).FirstOrDefault();
            var toCates = _context.BlogCategories.Where(e => e.BlogCategoryId != id && e.BlogId == fromCategory.BlogId).ToList();
            ViewData["Cates"] = new SelectList(toCates, "BlogCategoryId", "Title");
            return View(fromCategory);
        }

        [Authorize(BlogApplicationAuthorizationPolicies.EditBlogCategoryPolicy)]
        [HttpPost]
        public async Task<IActionResult> MergeCategory(int id, int toId)
        {
            var fromCategory = _context.BlogCategories.Where(e => e.BlogCategoryId == id).FirstOrDefault();
            var result = await MergeCategoryAsync(id, toId);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            var toCates = _context.BlogCategories.Where(e => e.BlogCategoryId != id && e.BlogId == fromCategory.BlogId).ToList();
            ViewData["Cates"] = new SelectList(toCates, "BlogCategoryId", "Title", toId);
            return View(fromCategory);
        }

        /// <summary>
        /// Merge category to another and deactivate the old category.
        /// </summary>
        /// <param name="fromCategoryId"></param>
        /// <param name="toCategoryId"></param>
        private async Task<bool> MergeCategoryAsync(int fromCategoryId, int toCategoryId)
        {
            var fromCategory = await (from cate in _context.BlogCategories.Include(e => e.BlogPosts).ThenInclude(e => e.BlogPost)
                                      where cate.BlogCategoryId == fromCategoryId
                                      select cate).FirstOrDefaultAsync();
            var toCategory = await (from cate in _context.BlogCategories.Include(e => e.BlogPosts)
                                    where cate.BlogCategoryId == toCategoryId
                                    select cate).FirstOrDefaultAsync();

            // If the new category is in the same category as the old one.
            if (fromCategory.BlogId == toCategory.BlogId)
            {
                foreach (var post in fromCategory.BlogPosts)
                {
                    _context.Add(new BlogPostCategory { BlogCategoryId = toCategoryId, BlogPostId = post.BlogPostId });
                    _context.Remove(post);
                }
            }
            else
            {
                ModelState.AddModelError("toId", "Merging into category of another blog is not supported currently.");
                return false;
            }
            //else
            //{
            //    foreach (var post in fromCategory.BlogPosts)
            //    {
            //        post.BlogPost.BlogId = toCategory.BlogId;
            //        post.BlogPost.DateModified = DateTime.Now;
            //        _context.Update(post.BlogPost);

            //        foreach (var comment in post.BlogPost.Comments)
            //        {
            //            comment.BlogId = toCategory.BlogId;
            //            _context.Update(comment);
            //        }


            //        post.BlogCategoryId = toCategoryId;

            //        _context.Update(post);
            //    }
            //}

            // deactive the from category
            fromCategory.Active = false;
            fromCategory.DateModified = DateTime.Now;
            _context.Update(fromCategory);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
