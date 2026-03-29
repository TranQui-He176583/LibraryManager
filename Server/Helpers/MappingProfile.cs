using AutoMapper;
using Server.Models;
using Server.DTOs;

namespace Server.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDetailDTO>()
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.PublisherName : null))
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Select(a => new AuthorDTO { AuthorId = a.AuthorId, AuthorName = a.AuthorName })))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => new CategoryDTO { CategoryId = c.CategoryId, CategoryName = c.CategoryName })));
            
            CreateMap<User, LoginResponseDTO>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
        }
    }
}
