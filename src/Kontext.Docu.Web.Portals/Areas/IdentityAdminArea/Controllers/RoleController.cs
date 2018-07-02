using AutoMapper;
using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace Kontext.Docu.Web.Portals.Job.Areas.IdentityAdminArea.Controllers
{
    [Area("IdentityAdminArea")]
    [Route("admin/identity/roles")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IAccountManager accountManager;
        private readonly IAuthorizationService authorizationService;
        private readonly ILoggerFactory loggerFactory;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IContextUnitOfWork unitOfWork;

        public RoleController(IAccountManager accountManager, IAuthorizationService authorizationService, ILoggerFactory loggerFactory, UserManager<ApplicationUser> userManager, IContextUnitOfWork unitOfWork)
        {
            this.accountManager = accountManager;
            this.authorizationService = authorizationService;
            this.loggerFactory = loggerFactory;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Api to retrieve roles.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{page?}/{pageSize?}")]
        [Authorize(ApplicationAuthorizationPolicies.ViewRolesPolicy)]
        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var roles = await accountManager.GetRolesLoadRelatedAsync(page, pageSize);
            var roleList = Mapper.Map<List<RoleViewModel>>(roles);
            var pagedList = roleList.ToPagedList(page, pageSize);

            var pagedVM = new PagedViewModel<object, RoleViewModel>(null, pagedList, pagedList.TotalItemCount, pagedList.PageCount, pagedList.PageSize, pagedList.PageNumber);

            return View(pagedVM);
        }

        [HttpGet("edit")]
        [Authorize(ApplicationAuthorizationPolicies.ManageRolesPolicy)]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await accountManager.GetRoleLoadRelatedByIdAsync(id);
            var roleVM = Mapper.Map<RoleViewModel>(role);
            return View(roleVM);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        [Authorize(ApplicationAuthorizationPolicies.ManageRolesPolicy)]
        public async Task<IActionResult> Edit([FromForm] RoleViewModel roleViewModel, [FromForm]string[] permissions)
        {
            if (ModelState.IsValid)
            {

                ApplicationRole appRole = await accountManager.GetRoleByIdAsync(roleViewModel.Id);
                Mapper.Map(roleViewModel, appRole);

                var result = await accountManager.UpdateRoleAsync(appRole, permissions);
                if (result.Item1)
                    return RedirectToAction(nameof(List));
                else
                {
                    AddErrors(result.Item2);
                }
            }
            return View(roleViewModel);
        }

        [HttpGet("add")]
        [Authorize(ApplicationAuthorizationPolicies.ManageRolesPolicy)]
        public IActionResult Add(string id)
        {
            return View();
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        [Authorize(ApplicationAuthorizationPolicies.ManageRolesPolicy)]
        public async Task<IActionResult> Add([FromForm] RoleViewModel roleViewModel, [FromForm]string[] permissions)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole appRole = Mapper.Map<ApplicationRole>(roleViewModel);
                var result = await accountManager.CreateRoleAsync(appRole, permissions);
                if (result.Item1)
                    return RedirectToAction(nameof(List));
                else
                {
                    AddErrors(result.Item2);
                }
            }
            return View(roleViewModel);
        }


        /// <summary>
        /// Add errors to the model validation
        /// </summary>
        /// <param name="errors"></param>
        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}