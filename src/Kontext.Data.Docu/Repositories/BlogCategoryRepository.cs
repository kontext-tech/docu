using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class BlogCategoryRepository : Repository<BlogCategory>, IBlogCategoryRepository
    {
        public BlogCategoryRepository(DbContext context) : base(context)
        {
        }
    }
}
