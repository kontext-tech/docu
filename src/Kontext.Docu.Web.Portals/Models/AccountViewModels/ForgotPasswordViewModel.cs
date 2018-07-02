using System.ComponentModel.DataAnnotations;

namespace Kontext.Docu.Web.Portals.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        [Display(Name = nameof(Email))]
        public string Email { get; set; }
    }
}
