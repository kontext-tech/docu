using System.ComponentModel.DataAnnotations;

namespace Kontext.Docu.Web.Portals.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Display(Name = nameof(Captcha))]
        public string Captcha { get; set; }

        [Required(ErrorMessage = "You need to agree to our Cookie and privacy policy to proceed.")]
        [Display(Name = nameof(AgreeWithPrivacyPolicy))]
        public bool AgreeWithPrivacyPolicy { get; set; }
    }
}
