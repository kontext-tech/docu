namespace Kontext.Security.Blog
{
    public class BlogApplicationPermissionProvider : DefaultApplicationPermissionProvider
    {
        /// <summary>
        /// Blog admin permission group
        /// </summary>
        public const string BlogPermissionGroupName = "Blog Permissions";

        /// <summary>
        /// Manage blogs permission 
        /// </summary>
        public static ApplicationPermission ManageBlogs = new ApplicationPermission("Manage Blogs", "admin.blogs", BlogPermissionGroupName, "Permission to manage blogs.");

        /// <summary>
        /// View blogs permission
        /// </summary>
        public static ApplicationPermission ViewBlogs = new ApplicationPermission("View Blogs", "admin.blogs.view", BlogPermissionGroupName, "Permission to view blogs.");

        /// <summary>
        /// Manage blog categories permission 
        /// </summary>
        public static ApplicationPermission ManageBlogCategories = new ApplicationPermission("Manage Blog Categories", "admin.blogs.cates", BlogPermissionGroupName, "Permission to manage blog categories.");

        /// <summary>
        /// View blog categories permission
        /// </summary>
        public static ApplicationPermission ViewBlogCategories = new ApplicationPermission("View Blog Categories", "admin.blogs.cates.view", BlogPermissionGroupName, "Permission to view blog categories.");

        /// <summary>
        /// Manage blog posts permission 
        /// </summary>
        public static ApplicationPermission ManageBlogPosts = new ApplicationPermission("Manage Blog Posts", "admin.blogposts.cates", BlogPermissionGroupName, "Permission to manage blog posts.");

        /// <summary>
        /// View blog posts permission
        /// </summary>
        public static ApplicationPermission ViewBlogPosts = new ApplicationPermission("View Blog Posts", "admin.blogposts.view", BlogPermissionGroupName, "Permission to view blog posts.");

        /// <summary>
        /// Manage blog comments permission 
        /// </summary>
        public static ApplicationPermission ManageBlogComments = new ApplicationPermission("Manage Blog Comments", "admin.blogcomments.cates", BlogPermissionGroupName, "Permission to manage blog comments.");

        /// <summary>
        /// Manage blog tags permission 
        /// </summary>
        public static ApplicationPermission ManageTags = new ApplicationPermission("Manage Tags", "admin.tags.view", BlogPermissionGroupName, "Permission to manage tags.");

        /// <summary>
        /// View blog comments permission
        /// </summary>
        public static ApplicationPermission ViewBlogComments = new ApplicationPermission("View Blog Comments", "admin.blogcomments.view", BlogPermissionGroupName, "Permission to view blog comments.");

        public BlogApplicationPermissionProvider() : base()
        {
            AddPermission(ManageBlogs);
            AddPermission(ViewBlogs);

            AddPermission(ManageBlogCategories);
            AddPermission(ViewBlogCategories);

            AddPermission(ManageBlogPosts);
            AddPermission(ViewBlogPosts);

            AddPermission(ManageBlogComments);
            AddPermission(ViewBlogComments);

            AddPermission(ManageTags);
        }
    }
}
