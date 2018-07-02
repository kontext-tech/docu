using Kontext.Data.Repositories;

namespace Kontext.Data
{
    public sealed class ContextUnitOfWork : IContextUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public ContextUnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEmailMessageRepository EmailMessageRepository => new EmailMessageRepository(context);
        
        public int SaveChanges()
        {
            return context.SaveChanges();
        }
    }
}
