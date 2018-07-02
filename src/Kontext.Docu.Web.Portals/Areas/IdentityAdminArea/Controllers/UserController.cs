using AutoMapper;
using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Kontext.Docu.Web.Portals.Job.Areas.IdentityAdminArea.Controllers
{
    [Area("IdentityAdminArea")]
    [Route("admin/identity/users")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IAccountManager accountManager;
        private readonly IAuthorizationService authorizationService;
        private readonly ILoggerFactory loggerFactory;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IContextUnitOfWork unitOfWork;
        private readonly IStringLocalizer<ContextProjectSharedResource> stringLocalizer;

        public UserController(IAccountManager accountManager, IAuthorizationService authorizationService, ILoggerFactory loggerFactory, UserManager<ApplicationUser> userManager, IContextUnitOfWork unitOfWork, IStringLocalizer<ContextProjectSharedResource> stringLocalizer)
        {
            this.accountManager = accountManager;
            this.authorizationService = authorizationService;
            this.loggerFactory = loggerFactory;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Api to retrieve users.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{page?}/{pageSize?}")]
        [Authorize(ApplicationAuthorizationPolicies.ViewUsersPolicy)]
        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var usersAndRoles = await accountManager.GetUsersAndRolesAsync(page, pageSize);
            List<ApplicationUser> usersVM = new List<ApplicationUser>();
            foreach (var item in usersAndRoles)
            {
                //var userVM = Mapper.Map<UserViewModel>(item.Item1);
                //userVM.Roles = item.Item2;
                usersVM.Add(item.Item1);
            }
            var totalUserCount = await accountManager.GetUsersCountAsync();
            var pageCount = (int)Math.Ceiling(totalUserCount / (decimal)pageSize);

            var pagedVM = new PagedViewModel<object, ApplicationUser>(null, usersVM, totalUserCount, pageCount, pageSize, page);

            return View(pagedVM);
        }

        [HttpGet("search")]
        [Route("{page?}/{pageSize?}")]
        [Authorize(ApplicationAuthorizationPolicies.ViewUsersPolicy)]
        public async Task<IActionResult> Search(string keyword, int page = 1, int pageSize = 10)
        {
            ViewData["keyword"] = keyword;
            var usersAndRoles = await accountManager.SearchUsersAsync(keyword, page, pageSize);
            List<ApplicationUser> usersVM = new List<ApplicationUser>();
            foreach (var item in usersAndRoles.Item1)
            {
                usersVM.Add(item);
            }
            var totalUserCount = usersAndRoles.Item2;
            var pageCount = (int)Math.Ceiling(totalUserCount / (decimal)pageSize);

            var pagedVM = new PagedViewModel<object, ApplicationUser>(null, usersVM, totalUserCount, pageCount, pageSize, page);

            return View(pagedVM);
        }

        [HttpGet("edit")]
        [Authorize(ApplicationAuthorizationPolicies.ManageUsersPolicy)]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await accountManager.GetUserLoadRelatedByIdAsync(id);
            var userVM = Mapper.Map<UserViewModel>(user);
            if (User.Identity.Name == user.UserName)
            {
                AddErrors(new string[] { stringLocalizer["You are not allowed to modify your roles by policy."] });
                return RedirectToAction(nameof(List));
            }
            else
            {
                if (user.UserRoles != null)
                    userVM.Roles = user.UserRoles.Select(r => r.Role.Name).ToArray();
            }
            return View(userVM);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        [Authorize(ApplicationAuthorizationPolicies.ManageUsersPolicy)]
        public async Task<IActionResult> Edit([FromForm] UserViewModel userViewModel, [FromForm]string[] roles)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = await accountManager.GetUserByIdAsync(userViewModel.Id);
                Mapper.Map(userViewModel, appUser);

                if (!userViewModel.IsLockedOut)
                    appUser.LockoutEnd = null;
                var result = await accountManager.UpdateUserAsync(appUser, roles);
                if (result.Item1)
                    return RedirectToAction(nameof(List));
                else
                {
                    AddErrors(result.Item2);
                }
            }

            return View(userViewModel);
        }

        [HttpGet("add")]
        [Authorize(ApplicationAuthorizationPolicies.ManageUsersPolicy)]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        [Authorize(ApplicationAuthorizationPolicies.ManageUsersPolicy)]
        public async Task<IActionResult> Add([FromForm] UserViewModel userViewModel, [FromForm]string[] roles)
        {
            if (ModelState.IsValid)
            {
                if (roles == null || roles.Length == 0)
                {
                    AddErrors(new string[] { stringLocalizer["Roles cannot be empty."] });
                    return View(userViewModel);
                }
                ApplicationUser appUser = Mapper.Map<ApplicationUser>(userViewModel);
                var result = await accountManager.CreateUserAsync(appUser, roles, "pwd@123!#");
                if (result.Item1)
                    return RedirectToAction(nameof(List));
                else
                {
                    AddErrors(result.Item2);
                }
            }
            return View(userViewModel);
        }

        [HttpGet("detail")]
        [Authorize(ApplicationAuthorizationPolicies.ViewUsersPolicy)]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await accountManager.GetUserLoadRelatedByIdAsync(id);
            return View(user);
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