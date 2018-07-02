using Kontext.Data.Repositories;

namespace Kontext.Data
{
    public interface IContextBlogUnitOfWork : IUnitOfWork
    {
        IBlogRepository BlogRepository { get; }

        IBlogCategoryRepository BlogCategoryRepository { get; }

        IBlogMediaObjectRepository BlogMediaObjectRepository { get; }

        IBlogPostCategoryRespository BlogPostCategoryRespository { get; }

        IBlogPostRepository BlogPostRepository { get; }

        IBlogPostCommentRepository BlogPostCommentRepository { get; }

        IBlogPostTagRepository BlogPostTagRepository { get; }

        ILanguageRepository LanguageRepository { get; }

        ITagRepository TagRepository { get; }
    }
}
