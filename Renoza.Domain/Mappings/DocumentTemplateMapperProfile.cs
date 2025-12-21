using Newtonsoft.Json;
using Renoza.Domain.Entities.Documents;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class DocumentTemplateMapperProfile : AutoMapper.Profile
    {
        public DocumentTemplateMapperProfile()
        {
            CreateMap<DocumentTemplateDao, DocumentTemplate>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.TemplateTypeId, a => a.MapFrom(p => p.TemplateTypeId))
                .ForMember(p => p.TemplateTypeName, a => a.MapFrom(p => p.TemplateType.Name))
                .ForMember(p => p.Name, a => a.MapFrom(p => p.Name))
                .ForMember(p => p.Description, a => a.MapFrom(p => p.Description))
                .ForMember(p => p.FileUrl, a => a.MapFrom(p => p.FileUrl))
                .ForMember(p => p.FileSize, a => a.MapFrom(p => p.FileSize))
                .ForMember(p => p.AvailablePlaceholders, a => a.MapFrom(p =>
                    string.IsNullOrEmpty(p.AvailablePlaceholders)
                        ? null
                        : JsonConvert.DeserializeObject<List<string>>(p.AvailablePlaceholders)))
                .ForMember(p => p.CreatedBy, a => a.MapFrom(p => p.CreatedBy))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => p.UpdatedAt))
                ;
        }
    }
}
