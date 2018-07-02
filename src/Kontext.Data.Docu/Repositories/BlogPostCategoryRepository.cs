using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class BlogPostCategoryRepository : Repository<BlogPostCategory>, IBlogPostCategoryRespository
    {
        public BlogPostCategoryRepository(DbContext context) : base(context)
        {
        }
    }
}
