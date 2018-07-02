using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Kontext.Security.AuthorizationPolicies
{
    public class ViewUserByIdRequirement : IAuthorizationRequirement
    {

    }
    public class ViewUserByIdHandler : AuthorizationHandler<ViewUserByIdRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewUserByIdRequirement requirement, string targetUserName)
        {
            if (context.User.HasClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.ViewUsers) || AuthorizationHelper.IsSameUser(context.User, targetUserName))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }

    }
}
