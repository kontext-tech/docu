namespace Kontext.Configuration
{
    public enum DatabaseType
    {
        SQLServer,
        SQLite
    }

    public interface IDatabaseConfig
    {
        DatabaseType CoreDbType { get; set; }
        DatabaseType DocuDbType { get; set; }
    }
}
