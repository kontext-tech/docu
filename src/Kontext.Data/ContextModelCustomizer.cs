using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using JetBrains.Annotations;

namespace Kontext.Data
{
    public sealed class ContextModelCustomizer : RelationalModelCustomizer
    {

        public ContextModelCustomizer([NotNull] ModelCustomizerDependencies dependencies) : base(dependencies)
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

            // Register the Context project entity sets.
            builder.UseContextProjectShared();

            base.Customize(builder, context);

        }
    }
}
