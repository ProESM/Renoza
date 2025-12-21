using Renoza.Domain.Entities.Documents;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class DocumentTemplateTypeMapperProfile : AutoMapper.Profile
    {
        public DocumentTemplateTypeMapperProfile()
        {
            CreateMap<DocumentTemplateTypeDao, DocumentTemplateType>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.Name, a => a.MapFrom(p => p.Name))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                ;
        }
    }
}
