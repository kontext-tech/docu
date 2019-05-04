namespace Kontext.Configuration
{
    public class DatabaseConfig : IDatabaseConfig
    {
        public DatabaseConfig()
        {
            CoreDbType = DatabaseType.SQLServer;
            DocuDbType = DatabaseType.SQLServer;
        }

        public DatabaseType CoreDbType { get; set; }
        public DatabaseType DocuDbType { get; set; }
    }
}
