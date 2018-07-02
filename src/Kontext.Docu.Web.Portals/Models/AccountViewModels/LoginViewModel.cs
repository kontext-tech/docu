using System.ComponentModel.DataAnnotations;

namespace Kontext.Docu.Web.Portals.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress(ErrorMessage ="Email address is not valid.")]
        [Display(Name = nameof(Email))]
        public string Email { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Password))]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Display(Name = nameof(Captcha))]
        public string Captcha { get; set; }
    }
}
