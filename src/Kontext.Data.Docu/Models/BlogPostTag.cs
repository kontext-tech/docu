using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public sealed class BlogPostTag
    {
        [Key]
        public int BlogPostId { get; set; }

        [Key]
        public int TagId { get; set; }

        public BlogPost BlogPost { get; set; }

        public Tag Tag { get; set; }

    }
}
