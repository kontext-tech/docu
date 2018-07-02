using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Logging;
using Kontext.Security;
using Kontext.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kontext.Docu.Web.Portals.Areas.BlogArea.Controllers
{
    [Produces("application/json")]
    [Route("api/BlogPostComment")]
    [Area("BlogArea")]
    public class BlogPostCommentController : Controller
    {
        private readonly IContextBlogUnitOfWork unitOfWork;
        private readonly IStringLocalizer<ContextProjectSharedResource> stringLocalizer;
        private readonly IConfigService configService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSenderService emailSender;
        private readonly ILoggerFactory loggerFactory;

        public BlogPostCommentController(IContextBlogUnitOfWork unitOfWork, IStringLocalizer<ContextProjectSharedResource> stringLocalizer, IConfigService configService, UserManager<ApplicationUser> userManager, IEmailSenderService emailSender, ILoggerFactory loggerFactory)
        {
            this.unitOfWork = unitOfWork;
            this.stringLocalizer = stringLocalizer;
            this.configService = configService;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.loggerFactory = loggerFactory;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddCommentAsync([FromBody] BlogPostCommentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!viewModel.AgreeWithPrivacyPolicy)
                {
                    ModelState.AddModelError(nameof(BlogPostCommentViewModel.AgreeWithPrivacyPolicy), stringLocalizer["You need to agree to our Cookie and privacy policy to proceed."]);
                    return Json(new { IsSuccessful = false, ErrorMessage = stringLocalizer["You need to agree to our Cookie and privacy policy to proceed."].Value });
                }
                var model = AutoMapper.Mapper.Map<BlogPostComment>(viewModel);
                model.Approved = configService.BlogConfig.AutoApproveComment;
                var currentDate = DateTime.Now;
                model.DateCreated = currentDate;
                model.DateModified = currentDate;
                model.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                model.IsDeleted = false;

                model.UserAgent = Request.Headers["User-Agent"].ToString();

                if (User.Identity.IsAuthenticated)
                {
                    try
                    {

                        var member = await userManager.FindByNameAsync(User.Identity.Name);
                        model.Author = member.FullName ?? member.UserName;
                        model.Email = member.Email;
                        model.UserId = Guid.Parse(member.Id);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { IsSuccessful = false, ErrorMessage = ex.Message });
                    }
                }

                try
                {
                    unitOfWork.BlogPostCommentRepository.Add(model);
                    var post = await unitOfWork.BlogPostRepository.FindOneAsync(b => b.BlogPostId == model.BlogPostId);
                    if (post != null)
                    {
                        post.CommentCount += 1;
                        post.DateModified = DateTime.Now;
                    }
                    unitOfWork.SaveChanges();
                    await SendEmailAsync(model);
                    return Json(new { IsSuccessful = true, Title = model.Title });
                }
                catch (Exception ex)
                {
                    return Json(new { IsSuccessful = false, ErrorMessage = ex.Message });
                }
            }
            return Json(new { IsSuccessful = false, ErrorMessage = "Invalid model state." });
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="model"></param>
        private async Task SendEmailAsync(BlogPostComment model)
        {
            string toUserEmail = "";
            string toUser = "";
            string bccEmail = null;
            string title = model.Title;

            string commentUser = model.Author;

            var blog = await unitOfWork.BlogRepository.FindOneAsync(b => b.BlogId == model.BlogId.Value);
            var blogPost = await unitOfWork.BlogPostRepository.FindOneAsync(p => p.BlogPostId == model.BlogPostId.Value);
            string commentLink = Url.BlogPostNewCommentLink(blogName: blog.UniqueName, postName: blogPost.UniqueName, year: blogPost.DateCreated.Year, month: blogPost.DateCreated.Month, day: blogPost.DateCreated.Day, scheme: Request.Scheme, blogPostCommentId: model.BlogPostCommentId, host: Request.Host.ToString());

            //Repliy to comment
            if (model.ReplyToBlogPostCommentId.HasValue)
            {
                var replyToComment = await unitOfWork.BlogPostCommentRepository.FindOneAsync(c => c.BlogPostCommentId == model.ReplyToBlogPostCommentId.Value);
                if (replyToComment != null)
                {
                    toUser = replyToComment.Author;
                    toUserEmail = replyToComment.Email;
                }
                bccEmail = model.BlogPost.Email;
            }
            else // reply to the post directly
            {
                toUser = model.BlogPost.Author;
                toUserEmail = model.BlogPost.Email;
            }

            try
            {
                await emailSender.SendEmailFromTemplateAsync(toUser: toUser, toUserEmail: toUserEmail, bccEmail: bccEmail, title: title, templateName: "Email.New.Comment", values: new(string key, string value)[] { ("link", commentLink), ("title", title), ("userName", toUser), ("commentContent", model.Text), ("commentUser", model.Author) });
            }
            catch (Exception ex)
            {
                if (loggerFactory != null)
                    loggerFactory.CreateLogger<EmailSenderService>().LogError(LoggingEvents.SEND_EMAIL_ERROR, ex, "An error occurred while sending email");
            }
        }
    }
}