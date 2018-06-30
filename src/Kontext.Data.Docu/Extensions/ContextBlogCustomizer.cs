using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using JetBrains.Annotations;
using Kontext.Data.MasterDataModels.Extensions;

namespace Kontext.Data
{
    public sealed class ContextBlogCustomizer : RelationalModelCustomizer
    {

        public ContextBlogCustomizer([NotNull] ModelCustomizerDependencies dependencies) : base(dependencies)
        {
        }

        public override void Customize(ModelBuilder builder, DbContext context)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Register the Context data models entity sets.
            builder.UseContextBlogModels();

            base.Customize(builder, context);

        }
    }
}
