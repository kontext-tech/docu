using System.Threading.Tasks;

namespace Kontext.Data
{
    /// <summary>
    /// Interface for database initializer: provide task to intialize database in DEV environment.
    /// </summary>
    public interface IDatabaseInitializer
    {

        Task SeedAsync();

    }
}
