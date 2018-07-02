using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public class BlogPost
    {
        [Key]
        public int BlogPostId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [MaxLength(64)]
        public string Author { get; set; }

        [MaxLength(64)]
        public string Email { get; set; }

        [MaxLength(16)]
        public string LanguageCode { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [MaxLength(128)]
        public string KeyWords { get; set; }

        public string Text { get; set; }

        public int? ViewCount { get; set; }

        public int? CommentCount { get; set; }

        [MaxLength(128)]
        public string UniqueName { get; set; }

        public string IpAddress { get; set; }

        [MaxLength(128)]
        public string Tag { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public DateTime? DateDeleted { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

        public DateTime? DatePublished { get; set; }

        [Required]
        public int? BlogId { get; set; }

        public virtual Blog Blog { get; set; }

        public virtual ICollection<BlogPostCategory> BlogCategories { get; set; }

        public virtual ICollection<BlogPostComment> Comments { get; set; }

        public virtual ICollection<BlogPostTag> Tags { get; set; }

        public virtual ICollection<BlogMediaObject> MediaObjects { get; set; }

        public virtual Language Language { get; set; }
    }
}
