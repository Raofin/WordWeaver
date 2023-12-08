using AutoMapper;
using WordWeaver.Data.Entity;
using WordWeaver.Dtos;
using WordWeaver.Extensions;

namespace WordWeaver.Mappings;

public class BlogProfiles : Profile
{
    public BlogProfiles()
    {
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds.ToLongList()));
        CreateMap<PostDto, Post>()
            .ForMember(dest => dest.PostId, opt => opt.Ignore())
            .ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds.ToCommaSeparatedString()));
        CreateMap<Comment, CommentFetchDto>();
    }
}
