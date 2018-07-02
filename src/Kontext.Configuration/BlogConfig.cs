namespace Kontext.Configuration
{
    public class BlogConfig : IBlogConfig
    {
        public int BlogPostCountPerPage { get; set; }
        public int BlogLatestPostCount { get; set; }
        public int BlogLatestCommentCount { get; set; }
        public bool AllowComments { get; set; }
        public int RssItemsCount { get; set; }
        public int HomePageBlogLatestPostCount { get; set; }
        public int HomePageBlogLatestCommentCount { get; set; }
        public bool AutoApproveComment { get; set; }
        public string DefaultLanguageLocale { get; set; }
        public char KeyWordsSeperator { get; set; }
    }
}
