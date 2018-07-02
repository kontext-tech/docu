using Kontext.Security;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Kontext.Docu.Web.Portals.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Convert a date time to timeago
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToTimeAgo(this DateTime dateTime)
        {
            TimeSpan span = DateTime.Now - dateTime;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("about {0} {1} ago",
                years, years == 1 ? "year" : "years");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("about {0} {1} ago",
                months, months == 1 ? "month" : "months");
            }
            if (span.Days > 0)
                return String.Format("about {0} {1} ago",
                span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return String.Format("about {0} {1} ago",
                span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return String.Format("about {0} {1} ago",
                span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            if (span.Seconds > 5)
                return String.Format("about {0} seconds ago", span.Seconds);
            if (span.Seconds <= 5)
                return "just now";
            return string.Empty;

        }

        /// <summary>
        /// Check whether datetime is 180 days ago
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsOld(this DateTime dateTime)
        {
            TimeSpan span = DateTime.Now - dateTime;
            if (span.TotalDays > 180)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether the idetity has certain permission claim.
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static bool HasPermissionClaim(this ClaimsPrincipal principal, string claim)
        {
            return principal.HasClaim(ApplicationClaimTypes.Permission, claim);
        }

        /// <summary>
        /// Find initials
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string ToInitials(this string fullName)
        {
            fullName = fullName.Trim();
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var names = fullName.Split(' ');
                int i = 0;
                var initials = new StringBuilder();

                while (i < 2 && i < names.Length)
                {
                    initials.Append(names[i][0]);
                    i++;
                }
                return initials.ToString().ToUpper();
            }
            return fullName;
        }

        static Random random = new Random();
        public static int GetRandomNumber(this int max)
        {
            return random.Next(0, max - 1);

        }

        private static readonly Regex TagRegex = new Regex(@"<[^>]*>?", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Convert page text to descript
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToPageDescription(this string text)
        {
            var desc = text.Length > 300 ? text.Substring(0, 300) : text;
            return TagRegex.Replace(desc, " ") + "...";
        }

        /// <summary>
        /// Convert text to masked text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToMaskedText(this string text)
        {
            if (text.Length >= 3)
                return text.Substring(0, Math.Max(text.Length / 3, 2)) + "***";
            else
                return "***";
        }
    }
}
