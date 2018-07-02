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
    [Authorize(BlogApplicationAuthorizationPolicies.ManageTagsPolicy)]
    public class TagsController : Controller
    {
        private readonly ContextBlogDbContext _context;

        public TagsController(ContextBlogDbContext context)
        {
            _context = context;
        }

        // GET: BlogAdminArea/Tags
        public async Task<IActionResult> Index()
        {
            var contextBlogDbContext = _context.Tags.Include(t => t.Language);
            return View(await contextBlogDbContext.ToListAsync());
        }

        public async Task<IActionResult> RebuildNTile(int tileCount = 9)
        {
            var tags = from tag in _context.Tags.Include(t => t.BlogPosts)
                       let postCount = tag.BlogPosts.Count()
                       orderby postCount
                       select tag;
            var tagsCount = tags.Count();
            var tagsPerTile = tagsCount / tileCount;

            var currentTile = 0;
            var tagsCountInCurrentTile = 0;

            foreach (var tag in tags)
            {
                if (currentTile >= (tileCount - 1))
                    tag.NTile = tileCount - 1;
                else
                    tag.NTile = currentTile;

                tagsCountInCurrentTile++;

                if (tagsCountInCurrentTile >= tagsPerTile)
                {
                    currentTile++;
                    tagsCountInCurrentTile = 0;
                }

                _context.Update(tag);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: BlogAdminArea/Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .Include(t => t.Language)
                .SingleOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: BlogAdminArea/Tags/Create
        public IActionResult Create()
        {
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode");
            return View();
        }

        // POST: BlogAdminArea/Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TagId,TagName,LanguageCode,NTile,IsDeleted,DateDeleted,DateCreated,DateModified")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", tag.LanguageCode);
            return View(tag);
        }

        // GET: BlogAdminArea/Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.SingleOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", tag.LanguageCode);
            return View(tag);
        }

        // POST: BlogAdminArea/Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TagId,TagName,LanguageCode,NTile,IsDeleted,DateDeleted,DateCreated,DateModified")] Tag tag)
        {
            if (id != tag.TagId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.TagId))
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
            ViewData["LanguageCode"] = new SelectList(_context.Languages, "LanguageCode", "LanguageCode", tag.LanguageCode);
            return View(tag);
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }
    }
}
