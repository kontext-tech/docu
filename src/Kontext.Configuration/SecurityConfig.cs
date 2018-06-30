namespace Kontext.Configuration
{
    public class SecurityConfig : ISecurityConfig
    {
        public string UserRoleName { get; set; }
        public string AdministratorRoleName { get; set; }
    }
}
