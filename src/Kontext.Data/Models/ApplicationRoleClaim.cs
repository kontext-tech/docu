using Microsoft.AspNetCore.Identity;

namespace Kontext.Data.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole ApplicationRole { get; set; }
    }
}
