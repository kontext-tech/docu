using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    /// <summary>
    /// Base class for models that are localizable
    /// </summary>
    public abstract class LocalizableModel: GenericModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [Display(Name = nameof(ResourceName))]
        public string ResourceName { get; set; }
    }
}
