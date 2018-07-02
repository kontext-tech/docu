﻿namespace Kontext.Security
{
    /// <summary>
    /// All the policies defined in the application
    /// </summary>
    public class ApplicationAuthorizationPolicies
    {
        /// <summary>
        /// Policy to perform generic admin tasks
        /// </summary>
        public const string GenericAdminPolicy = "Generic Admin";

        ///<summary>Policy to allow viewing the current user or another user's records (Requires target userId as parameter).</summary>
        public const string ViewUserByUserIdPolicy = "View User by ID";

        ///<summary>Policy to allow viewing all user records.</summary>
        public const string ViewUsersPolicy = "View Users";

        ///<summary>Policy to allow updating the current user or managing other user records (Requires target userId as parameter).</summary>
        public const string ManageUserByUserIdPolicy = "Manage User by ID";

        ///<summary>Policy to allow adding, removing and updating other user records.</summary>
        public const string ManageUsersPolicy = "Manage Users";

        /// <summary>Policy to allow viewing details of the current user's role or other roles (Requires roleName as parameter).</summary>
        public const string ViewRoleByRoleNamePolicy = "View Role by RoleName";

        /// <summary>Policy to allow viewing details of all roles.</summary>
        public const string ViewRolesPolicy = "View Roles";

        /// <summary>Policy to allow assigning roles.</summary>
        public const string AssignRolesPolicy = "Assign Roles";

        /// <summary>Policy to allow adding, removing and updating roles.</summary>
        public const string ManageRolesPolicy = "Manage Roles";

        /// <summary>Policy to allow adding, removing and updating roles.</summary>
        public const string ManageEmailsPolicy = "Manage Emails";
    }
}
