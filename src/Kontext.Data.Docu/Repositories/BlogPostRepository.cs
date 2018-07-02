using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository(DbContext context) : base(context)
        {
        }
    }
}
