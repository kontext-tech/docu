using System.Collections.Generic;

namespace Kontext.Security
{
    public interface IApplicationPermissionProvider
    {
        List<ApplicationPermission> AllPermissions { get; }

        /// <summary>
        /// Return permission object via permission name
        /// </summary>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        ApplicationPermission GetPermissionByName(string permissionName);

        /// <summary>
        /// Returns permission object via permission value
        /// </summary>
        /// <param name="permissionValue"></param>
        /// <returns></returns>
        ApplicationPermission GetPermissionByValue(string permissionValue);

        /// <summary>
        /// Returns all permission values
        /// </summary>
        /// <returns></returns>
        string[] GetAllPermissionValues();

        /// <summary>
        /// Returns all administrative permission values
        /// </summary>
        /// <returns></returns>
        string[] GetAdministrativePermissionValues();

        ApplicationPermission[] GetAdministrativePermissions();

        /// <summary>
        /// Returns all non-administrative permission values
        /// </summary>
        /// <returns></returns>
        string[] GetNonAdministrativePermissionValues();

        ApplicationPermission[] GetNonAdministrativePermissions();

    }
}
