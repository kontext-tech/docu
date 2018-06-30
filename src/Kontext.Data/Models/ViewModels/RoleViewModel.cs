using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models.ViewModels
{
    /// <summary>
    /// View model for role
    /// </summary>
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = nameof(Name))]
        public string Name { get; set; }

        [Display(Name = nameof(Description))]
        public string Description { get; set; }

        public int UsersCount { get; set; }

        public PermissionViewModel[] Permissions { get; set; }
    }
}
