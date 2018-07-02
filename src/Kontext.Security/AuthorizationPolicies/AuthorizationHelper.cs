using System.Security.Claims;

namespace Kontext.Security.AuthorizationPolicies
{
    public class AuthorizationHelper
    {
        public static string GetUserId(ClaimsPrincipal user)
        {
            return user.Identity.Name?.Trim();
        }

        public static bool IsSameUser(ClaimsPrincipal user, string targetUserId)
        {
            return GetUserId(user) == targetUserId;
        }
    }
}
