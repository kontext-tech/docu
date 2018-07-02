using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Kontext.Data
{
    public static class DbContextOptionsBuilderExtensions
    {
        const string ContextProjectSchema = "context";
        /// <summary>
        /// Extension method to register context project entity sets.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseContextProjectShared(this DbContextOptionsBuilder builder)
        {
            ContextExtension extension = new ContextExtension();
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension<ContextExtension>(extension);

            return builder;
        }

        /// <summary>
        /// Extension method to build Context Project model.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder UseContextProjectShared(this ModelBuilder builder)
        {
            // Map table names
            builder.Entity<EmailMessage>().ToTable("EmailMessages", ContextProjectSchema);

            // Map relationships
            

            builder.Entity<EmailMessage>(entity =>
            {
                entity.HasKey(em => em.EmailMessageId);
            });
            
            return builder;
        }
    }
}
