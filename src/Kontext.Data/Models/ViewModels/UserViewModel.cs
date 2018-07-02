using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models.ViewModels
{
    /// <summary>
    /// View model for user
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Display(Name = "FullName")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public string Email { get; set; }

        [Display(Name = "IsEnabled")]
        public bool IsEnabled { get; set; }

        [Display(Name = "IsLockedOut")]
        public bool IsLockedOut { get; set; }

        [MinLength(1, ErrorMessage = "Roles cannot be empty.")]
        public string[] Roles { get; set; }
    }
}
