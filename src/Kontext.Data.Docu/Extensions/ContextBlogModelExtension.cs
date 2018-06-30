using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kontext.Data
{
    /// <summary>
    /// DbContext options extension of Context Project. 
    /// </summary>
    public class ContextBlogModelExtension : IDbContextOptionsExtension
    {
        public string LogFragment => null;

        public bool ApplyServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IModelCustomizer, ContextBlogCustomizer>();

            return false;

        }

        public long GetServiceProviderHashCode() => 0;

        public void Validate(IDbContextOptions options) { }
    }
}
