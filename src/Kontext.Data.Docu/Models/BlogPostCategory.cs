using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public class BlogPostCategory
    {
        [Key]
        [Required]
        public int BlogPostId { get; set; }

        [Key]
        [Required]
        public int BlogCategoryId { get; set; }

        public BlogPost BlogPost { get; set; }

        public BlogCategory BlogCategory { get; set; }
    }
}
