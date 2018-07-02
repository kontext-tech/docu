using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    /// <summary>
    /// Blog entity
    /// </summary>
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }

        public Guid? UserId { get; set; }

        public Guid? BlogGroupId { get; set; }

        [Required]
        [MaxLength(128)]
        public string Title { get; set; }

        [Required]
        [MaxLength(256)]
        public string SubTitle { get; set; }

        [Required]
        [MaxLength(128)]
        public string UniqueName { get; set; }

        public bool? IsActive { get; set; }

        [MaxLength(16)]
        public string LanguageCode { get; set; }

        [MaxLength(128)]
        public string SkinCssFile { get; set; }

        public string SecondaryCss { get; set; }

        public int? PostCount { get; set; }

        public int? CommentCount { get; set; }

        public int? FileCount { get; set; }

        public int? PingTrackCount { get; set; }

        public string News { get; set; }

        public string TrackingCode { get; set; }

        [MaxLength(128)]
        public string Tag { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

        public virtual ICollection<BlogCategory> Categories { get; set; }

        public virtual ICollection<BlogPost> Posts { get; set; }

        public virtual ICollection<BlogPostComment> Comments { get; set; }

        public virtual ICollection<BlogMediaObject> MediaObjects { get; set; }

        public virtual Language Language { get; set; }
    }

}
