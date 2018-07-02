using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class BlogRepository : Repository<Blog>, IBlogRepository
    {
        public BlogRepository(DbContext context) : base(context)
        {
        }
    }
}
