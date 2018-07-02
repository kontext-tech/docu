using System;
using System.ComponentModel.DataAnnotations;

namespace Kontext.Data.Models
{
    public sealed class EmailMessage
    {
        [Key]
        public int EmailMessageId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        public string MessageBody { get; set; }

        [Required]
        [MaxLength(128)]
        public string FromEmail { get; set; }

        [Required]
        [MaxLength(128)]
        public string FromEmailDisplayName { get; set; }

        [MaxLength(4000)]
        public string ToEmails { get; set; }

        [MaxLength(4000)]
        public string CCEmails { get; set; }

        [Required]
        public bool IsSent { get; set; }

        public bool? IsSuccessful { get; set; }

        public DateTime? DateSent { get; set; }

        public string Comment { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public DateTime? DateDeleted { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateModified { get; set; }
    }
}
