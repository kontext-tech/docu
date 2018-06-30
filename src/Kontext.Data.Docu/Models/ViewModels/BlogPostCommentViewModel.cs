using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models.ViewModels
{
    public class BlogPostCommentViewModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [MaxLength(256, ErrorMessage = "Maximum {0} characters allowed.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Display(Name = nameof(Text))]
        public string Text { get; set; }

        public int? BlogId { get; set; }

        public int? BlogPostId { get; set; }

        public int? ReplyToBlogPostCommentId { get; set; }

        [Required(ErrorMessage = "You need to agree to our privacy policy before registering.")]
        [Display(Name = nameof(AgreeWithPrivacyPolicy))]
        public bool AgreeWithPrivacyPolicy { get; set; }
    }
}
