using AutoMapper;
using Kontext.Data;
using Kontext.Data.Models.ViewModels;
using Kontext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kontext.Docu.Web.Portals.ViewComponents
{
    [ViewComponent(Name = "BlogList")]
    public class BlogListViewComponent : ViewComponent
    {
        private readonly IContextBlogUnitOfWork unitWork;
        private readonly ICacheManager cacheManager;
        private readonly ILogger<BlogListViewComponent> logger;

        public BlogListViewComponent(IContextBlogUnitOfWork unitWork, ICacheManager cacheManager, ILogger<BlogListViewComponent> logger)
        {
            this.unitWork = unitWork;
            this.cacheManager = cacheManager;
            this.logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Menu", bool includingNonActive = false)
        {
            var key = $"{nameof(BlogListViewComponent)}_{includingNonActive}";

            if (!cacheManager.TryGetValue(key, out IEnumerable<BlogViewModel> blogViewModels))
            {
                logger.LogCritical("Caching blog list");
                var blogs = await (from e in unitWork.BlogRepository.Entities.Include(b => b.Categories)
                                   where includingNonActive || (e.IsActive.HasValue && e.IsActive.Value)
                                   orderby e.Title
                                   select e).ToListAsync();

                blogViewModels = new Collection<BlogViewModel>();
                Mapper.Map(blogs, blogViewModels);
                // Save data in cache.
                cacheManager.SetAbsoluteExpiration(key, blogViewModels, TimeSpan.FromDays(1));
            }

            return View(viewName, blogViewModels);
        }

    }
}
