using Kontext.Data.Models;
using Kontext.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kontext.Data
{
    public sealed class AccountManager : IAccountManager
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IApplicationPermissionProvider applicationPermissionProvider;

        public AccountManager(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IApplicationPermissionProvider applicationPermissionProvider)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.applicationPermissionProvider = applicationPermissionProvider;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<Tuple<ApplicationUser, string[]>> GetUserAndRolesAsync(string userId)
        {
            var user = await context.Users
                .Include(u => u.UserRoles)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            var userRoleIds = user.UserRoles.Select(r => r.RoleId).ToList();

            var roles = await context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToArrayAsync();

            return Tuple.Create(user, roles);
        }

        public async Task<List<Tuple<ApplicationUser, string[]>>> GetUsersAndRolesAsync(int page, int pageSize)
        {
            IQueryable<ApplicationUser> usersQuery = context.Users
                .Include(u => u.UserRoles)
                .OrderBy(u => u.UserName);

            if (page != -1)
                usersQuery = usersQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                usersQuery = usersQuery.Take(pageSize);

            var users = await usersQuery.ToListAsync();

            var userRoleIds = users.SelectMany(u => u.UserRoles.Select(r => r.RoleId)).ToList();

            var roles = await context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();

            return users.Select(u => Tuple.Create(u,
                roles.Where(r => u.UserRoles.Select(ur => ur.RoleId).Contains(r.Id)).Select(r => r.Name).ToArray()))
                .ToList();
        }

        public async Task<Tuple<bool, string[]>> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            user = await userManager.FindByNameAsync(user.UserName);

            try
            {
                if (roles != null)
                    result = await this.userManager.AddToRolesAsync(user, roles.Distinct());
            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdateUserAsync(ApplicationUser user)
        {
            return await UpdateUserAsync(user, null);
        }

        public async Task<Tuple<bool, string[]>> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            if (roles != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var rolesToRemove = userRoles.Except(roles).ToArray();
                var rolesToAdd = roles.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Any())
                {
                    result = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (rolesToAdd.Any())
                {
                    result = await userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());

            return Tuple.Create(true, new string[] { });
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (!await userManager.CheckPasswordAsync(user, password))
            {
                if (!userManager.SupportsUserLockout)
                    await userManager.AccessFailedAsync(user);

                return false;
            }

            return true;
        }

        public async Task<bool> TestCanDeleteUserAsync(string userId)
        {
            // TODO: check whether the user can be deleted directly
            await Task.Run(() => { return true; });
            return true;
        }

        public async Task<Tuple<bool, string[]>> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user != null)
                return await DeleteUserAsync(user);

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> DeleteUserAsync(ApplicationUser user)
        {
            var result = await userManager.DeleteAsync(user);
            return Tuple.Create(result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<ApplicationRole> GetRoleByIdAsync(string roleId)
        {
            return await roleManager.FindByIdAsync(roleId.ToString());
        }

        public async Task<ApplicationRole> GetRoleByNameAsync(string roleName)
        {
            return await roleManager.FindByNameAsync(roleName);
        }

        public async Task<ApplicationRole> GetRoleLoadRelatedAsync(string roleName)
        {
            var role = await context.Roles
                .Include(r => r.Claims)
                //.Include(r => r.UserRoles)
                .Where(r => r.Name == roleName)
                .FirstOrDefaultAsync();

            return role;
        }

        public async Task<ApplicationRole> GetRoleLoadRelatedByIdAsync(string roleId)
        {
            var role = await context.Roles
                .Include(r => r.Claims)
                //.Include(r => r.UserRoles)
                .Where(r => r.Id == roleId)
                .FirstOrDefaultAsync();

            return role;
        }

        public async Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<ApplicationRole> rolesQuery = context.Roles
                .Include(r => r.Claims)
                // .Include(r => r.UserRoles)
                .OrderBy(r => r.Name);

            if (page != -1)
                rolesQuery = rolesQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                rolesQuery = rolesQuery.Take(pageSize);

            var roles = await rolesQuery.ToListAsync();

            return roles;
        }

        public async Task<Tuple<bool, string[]>> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims)
        {
            if (claims == null)
                claims = new string[] { };

            string[] invalidClaims = claims.Where(c => applicationPermissionProvider.GetPermissionByValue(c) == null).ToArray();
            if (invalidClaims.Any())
                return Tuple.Create(false, new string[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });


            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            role = await roleManager.FindByNameAsync(role.Name);

            foreach (string claim in claims.Distinct())
            {
                result = await this.roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, applicationPermissionProvider.GetPermissionByValue(claim)));

                if (!result.Succeeded)
                {
                    await DeleteRoleAsync(role);
                    return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdateRoleAsync(ApplicationRole role, IEnumerable<string> claims)
        {
            if (claims != null)
            {
                string[] invalidClaims = claims.Where(c => applicationPermissionProvider.GetPermissionByValue(c) == null).ToArray();
                if (invalidClaims.Any())
                    return Tuple.Create(false, new string[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });
            }


            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            if (claims != null)
            {
                var roleClaims = (await roleManager.GetClaimsAsync(role)).Where(c => c.Type == ApplicationClaimTypes.Permission);
                var roleClaimValues = roleClaims.Select(c => c.Value).ToArray();

                var claimsToRemove = roleClaimValues.Except(claims).ToArray();
                var claimsToAdd = claims.Except(roleClaimValues).Distinct().ToArray();

                if (claimsToRemove.Any())
                {
                    foreach (string claim in claimsToRemove)
                    {
                        result = await roleManager.RemoveClaimAsync(role, roleClaims.Where(c => c.Value == claim).FirstOrDefault());
                        if (!result.Succeeded)
                            return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }

                if (claimsToAdd.Any())
                {
                    foreach (string claim in claimsToAdd)
                    {
                        result = await roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, applicationPermissionProvider.GetPermissionByValue(claim)));
                        if (!result.Succeeded)
                            return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<bool> TestCanDeleteRoleAsync(string roleId)
        {
            return !await context.UserRoles.Where(r => r.RoleId == roleId).AnyAsync();
        }

        public async Task<Tuple<bool, string[]>> DeleteRoleAsync(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role != null)
                return await DeleteRoleAsync(role);

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> DeleteRoleAsync(ApplicationRole role)
        {
            var result = await roleManager.DeleteAsync(role);
            return Tuple.Create(result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<int> GetUsersCountAsync()
        {
            return await context.Users.CountAsync();
        }

        public async Task<int> GetRolesCountAsync()
        {
            return await context.Roles.CountAsync();
        }

        public async Task<ApplicationUser> GetUserLoadRelatedByIdAsync(string userId)
        {
            var user = await context.Users
                .Include(r => r.Claims)
                .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(r => r.Id == userId)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<ApplicationUser> GetRoleLoadRelatedByUserNameAsync(string userName)
        {
            var user = await context.Users
                .Include(r => r.Claims)
                .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(r => r.UserName == userName)
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<Tuple<List<ApplicationUser>, int>> SearchUsersAsync(string keyWord, int page, int pageSize)
        {
            var normalizedKeyWord = keyWord.ToUpper();
            IQueryable<ApplicationUser> userQuery = from u in context.Users
            .Include(r => r.Claims)
            .Include(r => r.UserRoles)
            .ThenInclude(ur => ur.Role)
                                                    where u.NormalizedUserName.StartsWith(normalizedKeyWord)
                                                    || u.NormalizedUserName.EndsWith(normalizedKeyWord)
                                                    || u.NormalizedEmail.StartsWith(normalizedKeyWord)
                                                    || u.NormalizedEmail.EndsWith(normalizedKeyWord)
                                                    orderby u.UserName
                                                    select u;

            int totalCount = await userQuery.CountAsync();
            if (page != -1)
                userQuery = userQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                userQuery = userQuery.Take(pageSize);

            var users = await userQuery.ToListAsync();
            return new Tuple<List<ApplicationUser>, int>(users, totalCount);
        }

        public async Task<Tuple<bool, string[]>> DisableUserAsync(ApplicationUser user)
        {
            user.IsEnabled = false;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> EnableUserAsync(ApplicationUser user)
        {
            user.IsEnabled = true;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
            return Tuple.Create(true, new string[] { });
        }
    }
}
