using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace Kontext.Docu.Web.Portals
{
    public static class ManageNavPages
    {
        public static string ActivePageKey => "ActivePage";
        public static string ActiveCategoryKey => "ActiveCategory";

        public static string GenericClass(ViewContext viewContext, string menuName) => PageNavClass(viewContext, menuName);

        #region Generic
        public static string Index => "Index";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        #endregion

        #region Identity admin
        public static string PermissionList => "PermissionList";

        public static string ManageRoles => "ManageRoles";

        public static string ManageUsers => "ManageUsers";

        public static string PermissionListNavClass(ViewContext viewContext) => PageNavClass(viewContext, PermissionList);

        public static string ManageRolesNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageRoles);

        public static string ManageUsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageUsers);
        #endregion

        #region Self admin

        public static string ChangePassword => "ChangePassword";

        public static string ExternalLogins => "ExternalLogins";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        public static string MyComments => "MyComments";

        public static string MyCommentsClass(ViewContext viewContext) => PageNavClass(viewContext, MyComments);

        public static string DeletePersonalData => "DeletePersonalData";

        #endregion

        #region Job admin
        public static string JobAdminHome => "JobAdminHome";

        public static string JobAdminHomeNavClass(ViewContext viewContext) => PageNavClass(viewContext, JobAdminHome);

        public static string ManageLocations => "ManageLocations";
        public static string ManageLocationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageLocations);

        public static string ViewMasterData => "ViewMasterData";
        public static string ViewMasterDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, ViewMasterData);

        public static string ManageJobCategories => "ManageJobCategories";
        public static string ManageJobCategoriesNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageJobCategories);

        public static string ManageJobs => "ManageJobs";
        public static string ManageJobsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageJobs);
        #endregion

        #region Blog admin
        public static string ManageBlogs => "ManageBlogs";
        public static string ManageBlogCategories => "ManageBlogCategories";
        public static string ManageBlogPosts => "ManageBlogPosts";
        public static string ManageBlogComments => "ManageBlogComments";
        public static string ManageTags => "ManageTags";

        #endregion

        public static string ManageEmails => "ManageEmails";
        public static string ManageCache => "ManageCache";
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData[ActivePageKey] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static string CategoryNavClass(ViewContext viewContext, string uniqueName)
        {
            var activeItem = viewContext.ViewData[ActiveCategoryKey] as string;
            return string.Equals(activeItem, uniqueName, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
        public static void AddActiveCategory(this ViewDataDictionary viewData, string uniqueName) => viewData[ActiveCategoryKey] = uniqueName;
    }
}
