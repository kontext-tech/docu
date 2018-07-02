using Kontext.Docu.Web.Portals.Controllers;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        // {blogName}/Archive/{year}/{month}/{day}/{postName}
        public static string BlogPostNewCommentLink(this IUrlHelper urlHelper, string blogName, string postName, int year, int month, int day, int blogPostCommentId, string scheme, string host)
        {
            var area = "BlogArea";
            return urlHelper.Action(
                action: "Index",
                controller: "BlogPost",
                values: new { blogName, postName, area },
                protocol: scheme,
                host: host,
                fragment: blogPostCommentId.ToString());
        }

        /// <summary>
        /// Generate blog post link
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="blogName"></param>
        /// <param name="postName"></param>
        /// <param name="datePublished"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static string BlogPostLink(this IUrlHelper urlHelper, string blogName, string postName, DateTime datePublished, string scheme)
        {
            var area = "BlogArea";
            int year = datePublished.Year;
            int month = datePublished.Month;
            int day = datePublished.Day;

            return urlHelper.Action(
                action: "Index",
                controller: "BlogPost",
                values: new { blogName, postName, area },
                protocol: scheme);
        }

        /// <summary>
        /// Home page link
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <returns></returns>
        public static string HomePageLink(this IUrlHelper urlHelper, string schema)
        {
            var area = "";
            return urlHelper.Action(
                action: "Index",
                controller: "Home",
                values: new { area },
                protocol: schema);
        }

        /// <summary>
        /// Blog link
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="blogName"></param>
        /// <returns></returns>
        public static string BlogLink(this IUrlHelper urlHelper, string blogName, string schema)
        {
            var area = "BlogArea";
            var name = blogName;
            return urlHelper.Action(
                action: "Detail",
                controller: "Home",
                values: new { name, area },
                protocol: schema);
        }

        public static string RssLink(this IUrlHelper urlHelper, string schema)
        {
            var area = "BlogArea";
            return urlHelper.Action(
                action: "Rss20AllAsync",
                controller: "Rss",
                values: new { area },
                protocol: schema);
        }

        public static string RssLink(this IUrlHelper urlHelper, int blogId, string schema)
        {
            var area = "BlogArea";
            return urlHelper.Action(
                action: "Rss20Async",
                controller: "Rss",
                values: new { area, blogId },
                protocol: schema);
        }
    }
}
