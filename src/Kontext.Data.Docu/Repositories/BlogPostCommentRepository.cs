using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class BlogPostCommentRepository : Repository<BlogPostComment>, IBlogPostCommentRepository

    {
        public BlogPostCommentRepository(DbContext context) : base(context)
        {
        }
    }
}
