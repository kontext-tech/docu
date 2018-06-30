using Kontext.Data.Repositories;

namespace Kontext.Data
{
    public interface IContextUnitOfWork : IUnitOfWork
    {
        IEmailMessageRepository EmailMessageRepository { get; }
    }
}
