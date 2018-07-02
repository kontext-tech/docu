using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Docu.Web.Portals.Models.ManageViewModels
{
    public class EnableAuthenticatorViewModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        [ReadOnly(true)]
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }
    }
}
