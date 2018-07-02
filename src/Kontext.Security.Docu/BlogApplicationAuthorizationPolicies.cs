namespace Kontext.Security
{
    /// <summary>
    /// Authorization policies for blog application
    /// </summary>
    public sealed class BlogApplicationAuthorizationPolicies : ApplicationAuthorizationPolicies
    {

        /// <summary>
        /// Policy to view blog
        /// </summary>
        public const string ViewBlogPolicy = "View Blog";

        /// <summary>
        /// Policy to add blog
        /// </summary>
        public const string AddBlogPolicy = "Add Blog";

        /// <summary>
        /// Policy to edit blog
        /// </summary>
        public const string EditBlogPolicy = "Edit Blog";


        /// <summary>
        /// Policy to view blog category
        /// </summary>
        public const string ViewBlogCategoryPolicy = "View Blog Category";

        /// <summary>
        /// Policy to add blog category
        /// </summary>
        public const string AddBlogCategoryPolicy = "Add Blog Category";

        /// <summary>
        /// Policy to edit blog category
        /// </summary>
        public const string EditBlogCategoryPolicy = "Edit Blog Category";

        /// <summary>
        /// Policy to view blog post
        /// </summary>
        public const string ViewBlogPostPolicy = "View Blog Post";

        /// <summary>
        /// Policy to add blog post
        /// </summary>
        public const string AddBlogPostPolicy = "Add Blog Post";

        /// <summary>
        /// Policy to edit blog post
        /// </summary>
        public const string EditBlogPostPolicy = "Edit Blog Post";

        /// <summary>
        /// Policy to view blog comment
        /// </summary>
        public const string ViewBlogCommentPolicy = "View Blog Comment";

        /// <summary>
        /// Policy to add blog comment
        /// </summary>
        public const string AddBlogCommentPolicy = "Add Blog Comment";

        /// <summary>
        /// Policy to edit blog comment
        /// </summary>
        public const string EditBlogCommentPolicy = "Edit Blog Comment";

        /// <summary>
        /// Policy to allow adding, removing and updating tags.
        /// </summary>
        public const string ManageTagsPolicy = "Manage Tags";


    }
}
