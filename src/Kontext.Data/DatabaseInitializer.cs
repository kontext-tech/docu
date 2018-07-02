using Kontext.Data.Models;
using Kontext.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kontext.Data
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext context;
        private readonly IAccountManager accountManager;
        private readonly IApplicationPermissionProvider applicationPermissionProvider;

        public DatabaseInitializer(ApplicationDbContext context, IAccountManager accountManager, IApplicationPermissionProvider applicationPermissionProvider)
        {
            this.context = context;
            this.accountManager = accountManager;
            this.applicationPermissionProvider = applicationPermissionProvider;
        }

        public virtual async Task SeedAsync()
        {
            await SeedUsersAndRolesAsync();
        }

        /// <summary>
        /// Seed users and roles
        /// </summary>
        /// <returns></returns>
        private async Task SeedUsersAndRolesAsync()
        {
            if (CheckTableExists<ApplicationUser>(context) && !await context.Users.AnyAsync())
            {
                // Only migrate for the first time. 
                await context.Database.MigrateAsync().ConfigureAwait(false);
                await EnsureRoleAsync("Administrator", "Default administrator", applicationPermissionProvider.GetAllPermissionValues());
                await EnsureRoleAsync("User", "Default user", new string[] { });

                await CreateUserAsync("admin", "P@ssw0rd", "Inbuilt Administrator", "admin@kontext.tech", "+61 (03) 0000-0000", new string[] { "Administrator"
});
                await CreateUserAsync("user", "P@ssw0rd", "Inbuilt Standard User", "user@kontext.tech", "+61 (03) 0000-0000", new string[] { "User" });

                await context.SaveChangesAsync();
            }
        }

        private static bool CheckTableExists<T>(DbContext db) where T : class
        {
            try
            {
                db.Set<T>().Count();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }


        private async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        {
            if ((await accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                var result = await this.accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Item1)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Item2)}");
            }

        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true
            };

            var result = await accountManager.CreateUserAsync(applicationUser, roles, password);

            if (!result.Item1)
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Item2)}");


            return applicationUser;
        }
    }
}
