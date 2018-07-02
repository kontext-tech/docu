using Kontext.Security;
using Kontext.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kontext.Docu.Web.Portals.Areas.IdentityAdminArea.Controllers
{
    [Area("IdentityAdminArea")]
    [Authorize(ApplicationAuthorizationPolicies.ManageEmailsPolicy)]
    public class CacheEntriesController : Controller
    {
        private readonly ICacheManager cacheManager;

        public CacheEntriesController(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public IActionResult Index()
        {
            return View(cacheManager.GetAllCachedEntries());

        }

        public IActionResult RefreshEntry(string id)
        {
            cacheManager.Remove(id);
            return RedirectToAction("Index");
        }


    }
}