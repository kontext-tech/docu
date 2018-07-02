using System;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public abstract class GenericModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [Display(Name = nameof(SysIsEffective))]
        public bool SysIsEffective { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Display(Name = nameof(SysIsDeleted))]
        public bool SysIsDeleted { get; set; }

        [Display(Name = nameof(SysDateDeletedUtc))]
        public DateTime? SysDateDeletedUtc { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Display(Name = nameof(SysDateCreatedUtc))]
        public DateTime SysDateCreatedUtc { get; set; }

        [Display(Name = nameof(SysDateModifiedUtc))]
        public DateTime? SysDateModifiedUtc { get; set; }
    }
}
