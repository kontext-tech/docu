using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class BlogMediaObjectRepository : Repository<BlogMediaObject>, IBlogMediaObjectRepository
    {
        public BlogMediaObjectRepository(DbContext context) : base(context)
        {
        }
    }
}
