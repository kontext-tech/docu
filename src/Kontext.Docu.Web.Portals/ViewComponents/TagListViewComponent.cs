using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontext.Docu.Web.Portals.ViewComponents
{
    [ViewComponent(Name = "TagList")]
    public class TagListViewComponent : ViewComponent
    {
        private readonly IContextBlogUnitOfWork unitWork;
        private readonly IConfigService configService;
        private readonly ICacheManager cacheManager;
        private readonly ILogger<TagListViewComponent> logger;

        public TagListViewComponent(IContextBlogUnitOfWork unitWork, IConfigService configService, ICacheManager cacheManager, ILogger<TagListViewComponent> logger)
        {
            this.unitWork = unitWork;
            this.configService = configService;
            this.cacheManager = cacheManager;
            this.logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default", bool includingNonActive = false)
        {
            var key = $"{nameof(TagListViewComponent)}_{viewName}_{includingNonActive}";

            if (!cacheManager.TryGetValue(key, out IEnumerable<Tag> tags))
            {
                logger.LogCritical("Caching blog list");
                var query = from t in unitWork.TagRepository.Entities
                            where (includingNonActive || t.IsDeleted == false)
                            select t;

                tags = await query.ToListAsync();

                // Save data in cache.
                cacheManager.SetAbsoluteExpiration(key, tags, TimeSpan.FromDays(1));
            }


            return View(viewName, tags);
        }

    }
}
