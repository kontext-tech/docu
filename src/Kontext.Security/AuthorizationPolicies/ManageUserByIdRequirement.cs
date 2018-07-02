using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Kontext.Security.AuthorizationPolicies
{
    public sealed class ManageUserByIdRequirement : IAuthorizationRequirement
    {

    }

    public class ManageUserByIdHandler : AuthorizationHandler<ManageUserByIdRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageUserByIdRequirement requirement, string userId)
        {
            if (context.User.HasClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.ManageUsers) || AuthorizationHelper.IsSameUser(context.User, userId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
