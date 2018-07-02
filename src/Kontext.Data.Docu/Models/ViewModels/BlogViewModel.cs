using System;
using System.Collections.Generic;

namespace Kontext.Data.Models.ViewModels
{
    public class BlogViewModel
    {

        public int BlogId { get; set; }

        public Guid? UserId { get; set; }

        public Guid? BlogGroupId { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string UniqueName { get; set; }

        public bool? IsActive { get; set; }

        public string LanguageCode { get; set; }

        public string SkinCssFile { get; set; }

        public string SecondaryCss { get; set; }

        public int? PostCount { get; set; }

        public int? CommentCount { get; set; }

        public int? FileCount { get; set; }

        public int? PingTrackCount { get; set; }

        public string News { get; set; }

        public string TrackingCode { get; set; }

        public string Tag { get; set; }

        public DateTime DateCreated { get; }

        public DateTime DateModified { get; set; }

        public virtual ICollection<BlogCategory> Categories { get; set; }

        public virtual ICollection<BlogPost> Posts { get; set; }

        public virtual ICollection<BlogPostComment> Comments { get; set; }

        public virtual ICollection<BlogMediaObject> MediaObjects { get; set; }

        public virtual Language Language { get; set; }
    }
}
