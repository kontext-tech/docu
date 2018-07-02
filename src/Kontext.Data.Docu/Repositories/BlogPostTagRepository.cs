using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class BlogPostTagRepository : Repository<BlogPostTag>, IBlogPostTagRepository
    {
        public BlogPostTagRepository(DbContext context) : base(context)
        {
        }
    }
}
