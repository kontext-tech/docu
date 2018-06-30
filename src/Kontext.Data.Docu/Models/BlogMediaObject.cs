using System;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public class BlogMediaObject
    {
        [Key]
        public int MediaObjectId { get; set; }

        [MaxLength(32)]
        public string TypeName { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Url { get; set; }

        public int? BlogPostId { get; set; }

        public int? BlogId { get; set; }

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

        public virtual BlogPost BlogPost { get; set; }

        public virtual Blog Blog { get; set; }

        public virtual Language Language { get; set; }

    }
}
