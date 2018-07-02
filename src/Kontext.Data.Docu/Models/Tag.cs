using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public class Tag
    {
        [Key]
        [Required]
        public int TagId { get; set; }

        [Required]
        [MaxLength(64)]
        public string TagName { get; set; }

        [MaxLength(16)]
        public string LanguageCode { get; set; }

        [Required]
        public int NTile { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public DateTime? DateDeleted { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

        public virtual ICollection<BlogPostTag> BlogPosts { get; set; }

        public virtual Language Language { get; set; }
    }
}
