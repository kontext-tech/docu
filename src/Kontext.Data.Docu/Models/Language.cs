using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public sealed class Language
    {
        [MaxLength(16)]
        [Required]
        [Key]
        public string LanguageCode { get; set; }

        [Required]
        [MaxLength(32)]
        public string DisplayName { get; set; }

        [MaxLength(128)]
        public string Tag { get; set; }

        public ICollection<BlogPost> BlogPosts { get; set; }

        public ICollection<Blog> Blogs { get; set; }

        public ICollection<BlogPostComment> Comments { get; set; }

        public ICollection<Tag> Tags { get; set; }
    }
}
