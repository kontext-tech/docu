namespace Kontext.Configuration
{
    public interface IBlogConfig
    {
        int BlogPostCountPerPage { get; set; }
        int BlogLatestPostCount { get; set; }
        int BlogLatestCommentCount { get; set; }
        bool AllowComments { get; set; }
        int RssItemsCount { get; set; }
        int HomePageBlogLatestPostCount { get; set; }
        int HomePageBlogLatestCommentCount { get; set; }
        bool AutoApproveComment { get; set; }
    }
}
