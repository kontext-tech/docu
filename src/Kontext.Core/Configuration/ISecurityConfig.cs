namespace Kontext.Configuration
{
    public interface ISecurityConfig
    {
        string UserRoleName { get; set; }
        string AdministratorRoleName { get; set; }
    }
}
