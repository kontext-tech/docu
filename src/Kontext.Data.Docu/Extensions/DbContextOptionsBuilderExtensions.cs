using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Kontext.Data.MasterDataModels.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        const string ContextProjectSchema = "context";
        /// <summary>
        /// Extension method to register context blog entity sets.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseContextBlogModels(this DbContextOptionsBuilder builder)
        {
            ContextBlogModelExtension extension = new ContextBlogModelExtension();
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);

            return builder;
        }

        /// <summary>
        /// Extension method to build Context blog models.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder UseContextBlogModels(this ModelBuilder builder)
        {
            // Map table names
            builder.Entity<Blog>().ToTable("Blogs", ContextProjectSchema);
            builder.Entity<BlogPost>().ToTable("BlogPosts", ContextProjectSchema);
            builder.Entity<BlogCategory>().ToTable("BlogCategories", ContextProjectSchema);
            builder.Entity<BlogPostComment>().ToTable("BlogPostComments", ContextProjectSchema);
            builder.Entity<BlogPostCategory>().ToTable("BlogPostCategories", ContextProjectSchema);
            builder.Entity<Tag>().ToTable("Tags", ContextProjectSchema);
            builder.Entity<BlogPostTag>().ToTable("BlogPostTags", ContextProjectSchema);
            builder.Entity<BlogMediaObject>().ToTable("BlogMediaObjects", ContextProjectSchema);
            builder.Entity<Language>().ToTable("Languages", ContextProjectSchema);

            builder.Entity<Blog>(entity =>
            {
                entity.HasKey(b => b.BlogId);

                entity.HasMany(b => b.Categories)
                .WithOne(c => c.Blog)
                .HasForeignKey(c => c.BlogId)
                .IsRequired(required: true);

                entity.HasMany(b => b.Comments)
                .WithOne(c => c.Blog)
                .HasForeignKey(c => c.BlogId)
                .IsRequired(required: false);

                entity.HasMany(b => b.Posts)
                .WithOne(p => p.Blog)
                .HasForeignKey(p => p.BlogId)
                .IsRequired(required: true);

                entity.HasIndex(b => b.UniqueName)
                .IsUnique(unique: true);
            });

            builder.Entity<BlogPost>(entity =>
            {
                entity.HasKey(p => p.BlogPostId);

                entity.HasMany(p => p.Comments)
                .WithOne(c => c.BlogPost)
                .HasForeignKey(c => c.BlogPostId)
                .IsRequired(required: true);

                entity.HasMany(p => p.BlogCategories);

                entity.HasMany(p => p.Tags);

                entity.HasIndex(p => p.UniqueName)
                .IsUnique(unique: true);
            });

            builder.Entity<BlogCategory>(entity =>
            {
                entity.HasKey(c => c.BlogCategoryId);

                entity.HasMany(c => c.BlogPosts);

                entity.HasIndex(c => c.UniqueName)
                .IsUnique(unique: true);
            });

            builder.Entity<BlogPostCategory>(entity =>
            {
                entity.HasOne(pc => pc.BlogCategory)
                .WithMany(c => c.BlogPosts)
                .HasForeignKey(pc => pc.BlogCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(pc => pc.BlogPost)
                .WithMany(p => p.BlogCategories)
                .HasForeignKey(pc => pc.BlogPostId)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasKey(pc => new { pc.BlogCategoryId, pc.BlogPostId });

            });

            builder.Entity<BlogPostComment>(entity =>
            {
                entity.HasKey(c => c.BlogPostCommentId);

                entity.HasMany(c => c.Comments)
                .WithOne(c => c.ReplyToBlogPostComment)
                .HasForeignKey(c => c.ReplyToBlogPostCommentId)
                .IsRequired(required: false);
            });

            builder.Entity<Tag>(entity =>
            {
                entity.HasKey(t => t.TagId);

                entity.HasMany(t => t.BlogPosts);

                entity.HasIndex(t => t.TagName)
                .IsUnique(unique: true);
            });

            builder.Entity<BlogPostTag>(entity =>
            {
                entity.HasKey(pt => new { pt.BlogPostId, pt.TagId });

                entity.HasOne(pt => pt.Tag)
                .WithMany(t => t.BlogPosts)
                .HasForeignKey(pt => pt.TagId);

                entity.HasOne(pt => pt.BlogPost)
                .WithMany(p => p.Tags)
                .HasForeignKey(pt => pt.BlogPostId);

            });

            builder.Entity<BlogMediaObject>(entity =>
            {
                entity.HasKey(mo => mo.MediaObjectId);

                entity.HasOne(mo => mo.Blog)
                .WithMany(b => b.MediaObjects)
                .HasForeignKey(mo => mo.BlogId)
                .IsRequired(required: false);

                entity.HasOne(mo => mo.BlogPost)
                .WithMany(p => p.MediaObjects)
                .HasForeignKey(mo => mo.BlogPostId)
                .IsRequired(required: false);
            });

            builder.Entity<Language>(entity =>
            {
                entity.HasKey(l => l.LanguageCode);

                entity.HasMany(l => l.Blogs)
                .WithOne(b => b.Language)
                .HasForeignKey(b => b.LanguageCode)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(l => l.BlogPosts)
                .WithOne(b => b.Language)
                .HasForeignKey(b => b.LanguageCode)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(l => l.Comments)
                .WithOne(b => b.Language)
                .HasForeignKey(b => b.LanguageCode)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(l => l.Tags)
                .WithOne(b => b.Language)
                .HasForeignKey(b => b.LanguageCode)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.ClientSetNull);

            });

            return builder;
        }
    }
}
