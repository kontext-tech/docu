using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Services;
using Kontext.Docu.Web.Portals.Models.ManageViewModels;
using Kontext.Docu.Web.Portals.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using X.PagedList;

namespace Kontext.Docu.Web.Portals.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly IAccountManager accountManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSenderService _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IStringLocalizer<ContextProjectSharedResource> stringLocalizer;
        private readonly IConfigService configService;
        private readonly IContextBlogUnitOfWork unitOfWork;
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private readonly IRequestCultureFeature requestCulture;

        public ManageController(IAccountManager accountManager,
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSenderService emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          IStringLocalizer<ContextProjectSharedResource> stringLocalizer,
          IHttpContextAccessor contextAccessor,
          IConfigService configService,
          IContextBlogUnitOfWork unitOfWork)
        {
            this.accountManager = accountManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            this.stringLocalizer = stringLocalizer;
            this.configService = configService;
            this.unitOfWork = unitOfWork;
            requestCulture = contextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                //PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                FullName = user.FullName,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            //var phoneNumber = user.PhoneNumber;
            //if (model.PhoneNumber != phoneNumber)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
            //    }
            //}

            if (model.FullName != user.FullName)
            {
                user.FullName = model.FullName;
                var updateUserResult = await accountManager.UpdateUserAsync(user);
                if (!updateUserResult.Item1)
                {
                    throw new ApplicationException($"Unexpected error occurred setting user full name for user with ID '{user.Id}'.");
                }
            }

            StatusMessage = stringLocalizer["Your profile has been updated"];
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl, requestCulture.RequestCulture.UICulture.Name);

            StatusMessage = stringLocalizer["Verification email sent. Please check your email."];
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogCritical("User changed their password successfully.");
            StatusMessage = stringLocalizer["Your password has been changed."];

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = stringLocalizer["Your password has been set."];

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogCritical("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("model.Code", "Verification code is invalid.");
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogCritical("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToAction(nameof(GenerateRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogCritical("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            _logger.LogCritical("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return View(model);
        }

        [HttpGet]
        public IActionResult DeletePersonalData()
        {
            return View();
        }

        [HttpPost]
        [ActionName(nameof(DeletePersonalData))]
        public async Task<IActionResult> DeletePersonalDataConfirmedAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _logger.LogCritical("User with ID '{UserId}' asked for deletion of their personal data.", _userManager.GetUserId(User));

                var comments = await GetCommentsQuery(user).ToListAsync();
                foreach (var c in comments)
                {
                    if (!c.IsDeleted)
                    {
                        c.IsDeleted = true;
                        c.DateDeleted = DateTime.UtcNow;
                        unitOfWork.BlogPostCommentRepository.Update(c);
                    }
                }
                var result = await accountManager.DisableUserAsync(user);
                if (result.Item1)
                    unitOfWork.SaveChanges();
                else
                    throw new Exception(result.Item2[0]);

                ViewData["StatusMessage"] = stringLocalizer["User account is deactivated successfully!"].Value;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error happened when deleteing user data ({UserId}):{Error}.", _userManager.GetUserId(User), ex.ToString());
                ViewData["StatusMessage"] = stringLocalizer["Error: Please contact system administrtor!"].Value;
            }
            return View("DeletePersonalDataConfirmed");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            _logger.LogCritical("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, object>();
            personalData.Add(nameof(user.UserName), user.UserName?.ToString() ?? "null");
            personalData.Add(nameof(user.NormalizedUserName), user.NormalizedUserName?.ToString() ?? "null");
            personalData.Add(nameof(user.FullName), user.FullName?.ToString() ?? "null");
            personalData.Add(nameof(user.Email), user.Email?.ToString() ?? "null");
            personalData.Add(nameof(user.NormalizedEmail), user.NormalizedEmail?.ToString() ?? "null");
            personalData.Add(nameof(user.PhoneNumber), user.PhoneNumber?.ToString() ?? "null");
            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }
            /*Comments*/
            List<Tuple<int, string, string>> commentsData = new List<Tuple<int, string, string>>();
            var comments = await GetCommentsQuery(user).ToListAsync();
            foreach (var c in comments)
            {
                commentsData.Add(Tuple.Create(c.BlogPostCommentId, c.Title, c.Text));
            }
            personalData.Add("Comments", commentsData);
            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));
            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "text/json");
        }

        private IOrderedQueryable<BlogPostComment> GetCommentsQuery(ApplicationUser user)
        {
            var query = from comment in unitOfWork.BlogPostCommentRepository.Entities.Include(e => e.BlogPost).Include(e => e.Blog).Include(e => e.ReplyToBlogPostComment)
                        where ((comment.UserId.HasValue && comment.UserId == Guid.Parse(user.Id)) || comment.Email.ToUpper() == user.NormalizedEmail) && !comment.IsDeleted
                        orderby comment.DateModified descending
                        select comment;
            return query;
        }

        [HttpGet]
        public async Task<IActionResult> MyComments(int? page = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }
            var query = GetCommentsQuery(user);
            //TODO: change the post count per page to use context learning configurations
            var comments = await query.ToPagedListAsync(page, configService.BlogConfig.BlogPostCountPerPage);
            var pagedVM = new PagedViewModel<ApplicationUser, BlogPostComment>(user, comments, comments.TotalItemCount, comments.PageCount, comments.PageSize, comments.PageNumber);
            return View("_MyBlogComments", pagedVM);
        }

        /// <summary>
        /// Delete comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DeleteComment(int commentId, int? page = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(stringLocalizer["Unable to load user with ID {0}."], _userManager.GetUserId(User)));
            }

            var comment = unitOfWork.BlogPostCommentRepository.Get(commentId);
            if (comment != null && (comment.UserId.HasValue && comment.UserId == Guid.Parse(user.Id)) || comment.Email.ToUpper() == user.NormalizedEmail)
            {
                comment.IsDeleted = true;
                comment.DateDeleted = DateTime.UtcNow;
                unitOfWork.BlogPostCommentRepository.Update(comment);
                unitOfWork.SaveChanges();
                ViewData["StatusMessage"] = stringLocalizer["Comment is marked as deleted successfully!"].Value;
                return await MyComments(page);
            }
            else
            {
                ViewData["StatusMessage"] = stringLocalizer["Error: Action failed! Please contact system administrator if this happens again."].Value;
                return await MyComments(page);
            }
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("Kontext.Docu.Web.Portals"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion
    }
}
