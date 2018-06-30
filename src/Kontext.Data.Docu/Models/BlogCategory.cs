using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public class BlogCategory
    {
        [Key]
        public int BlogCategoryId { get; set; }

        [Required]
        [MaxLength(128)]
        public string UniqueName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        public bool Active { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [MaxLength(16)]
        public string LanguageCode { get; set; }

        [MaxLength(128)]
        public string Tag { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

        [Required]
        public int BlogId { get; set; }

        public virtual Blog Blog { get; set; }

        public virtual ICollection<BlogPostCategory> BlogPosts { get; set; }

        public virtual Language Language { get; set; }
    }
}
