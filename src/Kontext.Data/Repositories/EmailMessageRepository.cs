using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class EmailMessageRepository : Repository<EmailMessage>, IEmailMessageRepository
    {
        public EmailMessageRepository(DbContext context) : base(context)
        {
        }
    }
}
