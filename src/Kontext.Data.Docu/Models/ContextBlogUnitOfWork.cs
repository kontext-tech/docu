using Kontext.Data.Repositories;

namespace Kontext.Data
{
    public sealed class ContextBlogUnitOfWork : IContextBlogUnitOfWork
    {
        private readonly ContextBlogDbContext context;

        public ContextBlogUnitOfWork(ContextBlogDbContext context)
        {
            this.context = context;
        }

        public IBlogRepository BlogRepository => new BlogRepository(context);

        public IBlogCategoryRepository BlogCategoryRepository => new BlogCategoryRepository(context);

        public IBlogMediaObjectRepository BlogMediaObjectRepository => new BlogMediaObjectRepository(context);

        public IBlogPostCategoryRespository BlogPostCategoryRespository => new BlogPostCategoryRepository(context);

        public IBlogPostRepository BlogPostRepository => new BlogPostRepository(context);

        public IBlogPostCommentRepository BlogPostCommentRepository => new BlogPostCommentRepository(context);

        public IBlogPostTagRepository BlogPostTagRepository => new BlogPostTagRepository(context);

        public ILanguageRepository LanguageRepository => new LanguageRepository(context);

        public ITagRepository TagRepository => new TagRepository(context);

        public int SaveChanges()
        {
            return context.SaveChanges();
        }
    }
}
