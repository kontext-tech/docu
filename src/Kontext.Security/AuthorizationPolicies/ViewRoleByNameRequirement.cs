using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Kontext.Security.AuthorizationPolicies
{
    public class ViewRoleByNameRequirement : IAuthorizationRequirement
    {

    }

    public class ViewRoleByNameHandler : AuthorizationHandler<ViewRoleByNameRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewRoleByNameRequirement requirement, string roleName)
        {
            if (context.User.HasClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.ViewRoles) || context.User.IsInRole(roleName))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
