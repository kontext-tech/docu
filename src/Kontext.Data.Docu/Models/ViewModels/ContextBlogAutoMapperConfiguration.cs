using AutoMapper.EquivalencyExpression;

namespace Kontext.Data.Models.ViewModels
{
    public sealed class ContextBlogAutoMapperConfiguration: AutoMapperConfiguration
    {
        public ContextBlogAutoMapperConfiguration()
        {
            CreateMap<Blog, BlogViewModel>().ReverseMap().EqualityComparison((dto, o) =>
                dto.BlogId == dto.BlogId || dto.UniqueName == o.UniqueName);
            CreateMap<BlogPostComment, BlogPostCommentViewModel>().ReverseMap();
        }
    }
}
