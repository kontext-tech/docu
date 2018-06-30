using System.Collections.Generic;
using System.Linq;

namespace Kontext.Security
{
    public class DefaultApplicationPermissionProvider : IApplicationPermissionProvider
    {
        /// <summary>
        /// Generic admin permission group
        /// </summary>
        public const string OtherPermissionGroupName = "Other Permissions";

        /// <summary>
        /// Generic admin permission 
        /// </summary>
        public static ApplicationPermission GenericAdmin = new ApplicationPermission("Generic Admin", "other.genericadmin", OtherPermissionGroupName, "Permission to perform generic adminitrator tasks", true);

        /// <summary>
        /// Permission to manage emails
        /// </summary>
        public static ApplicationPermission ManageEmails = new ApplicationPermission("Manage Emails", "other.emails.admin", OtherPermissionGroupName, "Permission to manage emails", true);

        /// <summary>
        /// User permission group
        /// </summary>
        public const string UsersPermissionGroupName = "User Permissions";
        /// <summary>
        /// View user permission 
        /// </summary>
        public static ApplicationPermission ViewUsers = new ApplicationPermission("View Users", "users.view", UsersPermissionGroupName, "Permission to view other users account details");
        /// <summary>
        /// Manage user permission
        /// </summary>
        public static ApplicationPermission ManageUsers = new ApplicationPermission("Manage Users", "users.manage", UsersPermissionGroupName, "Permission to create, delete and modify other users account details", true);

        /// <summary>
        /// Role permission group
        /// </summary>
        public const string RolesPermissionGroupName = "Role Permissions";

        /// <summary>
        /// View roles permission 
        /// </summary>
        public static ApplicationPermission ViewRoles = new ApplicationPermission("View Roles", "roles.view", RolesPermissionGroupName, "Permission to view available roles");
        /// <summary>
        /// Manager role permission
        /// </summary>
        public static ApplicationPermission ManageRoles = new ApplicationPermission("Manage Roles", "roles.manage", RolesPermissionGroupName, "Permission to create, delete and modify roles", true);
        /// <summary>
        /// Asign role permission
        /// </summary>
        public static ApplicationPermission AssignRoles = new ApplicationPermission("Assign Roles", "roles.assign", RolesPermissionGroupName, "Permission to assign roles to users", true);

        /// <summary>
        /// All permissions defined in the application 
        /// </summary>
        private List<ApplicationPermission> allPermissions;

        public DefaultApplicationPermissionProvider()
        {
            allPermissions = new List<ApplicationPermission>()
            {
                // Other
                GenericAdmin,
                ManageEmails,

                // User Group
                ViewUsers,
                ManageUsers,

                // Role Group
                ViewRoles,
                ManageRoles,
                AssignRoles
            };
        }

        public void AddPermission(ApplicationPermission permission)
        {
            AllPermissions.Add(permission);
        }

        public List<ApplicationPermission> AllPermissions => allPermissions;

        public ApplicationPermission[] GetAdministrativePermissions() => AllPermissions.Where(e => e.IsAdministrative).ToArray();

        public string[] GetAdministrativePermissionValues() => GetAdministrativePermissions().Select(e => e.Value).ToArray();

        public string[] GetAllPermissionValues() => AllPermissions.Select(p => p.Value).ToArray();

        public ApplicationPermission[] GetNonAdministrativePermissions() => AllPermissions.Where(e => !e.IsAdministrative).ToArray();

        public string[] GetNonAdministrativePermissionValues() => GetNonAdministrativePermissions().Select(e => e.Value).ToArray();

        public ApplicationPermission GetPermissionByName(string permissionName) => AllPermissions.Where(p => p.Name == permissionName).FirstOrDefault();

        public ApplicationPermission GetPermissionByValue(string permissionValue) => AllPermissions.Where(p => p.Value == permissionValue).FirstOrDefault();
    }
}
