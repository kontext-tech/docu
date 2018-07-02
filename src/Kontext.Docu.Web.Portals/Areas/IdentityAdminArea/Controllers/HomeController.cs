using Kontext.Data;
using Kontext.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kontext.Docu.Web.Portals.Job.Areas.IdentityAdminArea.Controllers
{
    [Area("IdentityAdminArea")]
    [Route("admin/identity")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IApplicationPermissionProvider permissionProvider;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<HomeController> logger;
        private readonly IAccountManager accountManager;

        public HomeController(IApplicationPermissionProvider permissionProvider, IMemoryCache memoryCache, ILogger<HomeController> logger, IAccountManager accountManager)
        {
            this.permissionProvider = permissionProvider;
            this.memoryCache = memoryCache;
            this.logger = logger;
            this.accountManager = accountManager;
        }

        [Authorize(ApplicationAuthorizationPolicies.GenericAdminPolicy)]
        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync()
        {
            ViewData["PermissionCount"] = permissionProvider.AllPermissions.Count;

            var keyUserCount = $"{nameof(HomeController)}_UserCount";

            if (!memoryCache.TryGetValue(keyUserCount, out int userCount))
            {
                logger.LogCritical("Caching user count");

                // Key not in cache, so get data.
                userCount = await accountManager.GetUsersCountAsync();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromDays(1));

                // Save data in cache.
                memoryCache.Set(keyUserCount, userCount, cacheEntryOptions);
            }

            ViewData["UserCount"] = userCount;

            var keyRoleCount = $"{nameof(HomeController)}_RoleCount";

            if (!memoryCache.TryGetValue(keyRoleCount, out int roleCount))
            {
                logger.LogCritical("Caching role count");

                // Key not in cache, so get data.
                roleCount = await accountManager.GetRolesCountAsync();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromDays(1));

                // Save data in cache.
                memoryCache.Set(keyRoleCount, roleCount, cacheEntryOptions);
            }
            ViewData["RoleCount"] = roleCount;

            return View();
        }

        [Route("PermissionList")]
        [Authorize(ApplicationAuthorizationPolicies.GenericAdminPolicy)]
        public IActionResult PermissionList()
        {
            var permissions = permissionProvider.AllPermissions;
            return View(permissions);
        }
    }
}