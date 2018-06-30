using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsLockedOut => LockoutEnabled && this.LockoutEnd >= DateTimeOffset.UtcNow;

        public bool IsEnabled { get; internal set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        #region  asp.net core 2.0 support
        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        /// <summary>
        /// Navigation property for this users login accounts.6
        /// </summary>
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        #endregion

    }
}
