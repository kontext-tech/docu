using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data
{
    public class ContextBlogDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogPostCategory> BlogPostCategories { get; set; }
        public DbSet<BlogPostComment> BlogPostComments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogPostTag> BlogPostTags { get; set; }
        public DbSet<BlogMediaObject> BlogMediaObjects { get; set; }
        public DbSet<Language> Languages { get; set; }

        public ContextBlogDbContext(DbContextOptions<ContextBlogDbContext> options)
            : base(options)
        {
        }
    }
}
