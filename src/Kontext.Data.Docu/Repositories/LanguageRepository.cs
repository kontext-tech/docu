using Kontext.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontext.Data.Repositories
{
    public sealed class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        public LanguageRepository(DbContext context) : base(context)
        {
        }
    }
}
