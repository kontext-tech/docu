using System.ComponentModel.DataAnnotations;

namespace Kontext.Docu.Web.Portals.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = nameof(Username))]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Phone]
        //[Display(Name = "Phone number")]
        //public string PhoneNumber { get; set; }

        [MaxLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        public string StatusMessage { get; set; }
    }
}
