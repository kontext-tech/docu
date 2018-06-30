using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public class BlogPostComment
    {
        [Required]
        [Key]
        public int BlogPostCommentId { get; set; }

        [Required]
        public bool Approved { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [MaxLength(64)]
        public string Author { get; set; }

        [Required]
        public bool IsBlogUser { get; set; }

        public Guid? UserId { get; set; }

        [MaxLength(16)]
        public string LanguageCode { get; set; }

        [MaxLength(64)]
        public string Email { get; set; }

        public string Text { get; set; }

        [MaxLength(64)]
        public string IpAddress { get; set; }

        [MaxLength(150)]
        public string UserAgent { get; set; }

        [MaxLength(128)]
        public string Tag { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public DateTime? DateDeleted { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

        public int? BlogId { get; set; }

        public int? BlogPostId { get; set; }

        public int? ReplyToBlogPostCommentId { get; set; }

        public virtual Blog Blog { get; set; }

        public virtual BlogPost BlogPost { get; set; }

        public virtual BlogPostComment ReplyToBlogPostComment { get; set; }

        public virtual ICollection<BlogPostComment> Comments { get; set; }

        public virtual Language Language { get; set; }
    }
}
