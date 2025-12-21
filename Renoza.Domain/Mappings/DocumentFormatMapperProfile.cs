using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class DocumentFormatMapperProfile : AutoMapper.Profile
    {
        public DocumentFormatMapperProfile()
        {
            CreateMap<DocumentFormatDao, Entities.Documents.DocumentFormat>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.Name, a => a.MapFrom(p => p.Name))
                .ForMember(p => p.FileExtension, a => a.MapFrom(p => p.FileExtension))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                ;
        }
    }
}
