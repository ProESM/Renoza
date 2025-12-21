using Newtonsoft.Json;
using Renoza.Domain.Entities.Documents;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class DocumentMapperProfile : AutoMapper.Profile
    {
        public DocumentMapperProfile()
        {
            CreateMap<DocumentDao, Document>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.TemplateId, a => a.MapFrom(p => p.TemplateId))
                .ForMember(p => p.TemplateName, a => a.MapFrom(p => p.Template.Name))
                .ForMember(p => p.FormatId, a => a.MapFrom(p => p.FormatId))
                .ForMember(p => p.FormatName, a => a.MapFrom(p => p.Format.Name))
                .ForMember(p => p.FileExtension, a => a.MapFrom(p => p.Format.FileExtension))
                .ForMember(p => p.StatusId, a => a.MapFrom(p => p.StatusId))
                .ForMember(p => p.StatusName, a => a.MapFrom(p => p.Status.Name))
                .ForMember(p => p.Name, a => a.MapFrom(p => p.Name))
                .ForMember(p => p.FileUrl, a => a.MapFrom(p => p.FileUrl))
                .ForMember(p => p.FileSize, a => a.MapFrom(p => p.FileSize))
                .ForMember(p => p.PlaceholderData, a => a.MapFrom(p =>
                    string.IsNullOrEmpty(p.PlaceholderData)
                        ? null
                        : JsonConvert.DeserializeObject<Dictionary<string, string>>(p.PlaceholderData)))
                .ForMember(p => p.OrderId, a => a.MapFrom(p => p.OrderId))
                .ForMember(p => p.CustomerId, a => a.MapFrom(p => p.CustomerId))
                .ForMember(p => p.CreatedBy, a => a.MapFrom(p => p.CreatedBy))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => p.UpdatedAt))
                .ForMember(p => p.FirstDownloadedAt, a => a.MapFrom(p => p.FirstDownloadedAt))
                .ForMember(p => p.LastDownloadedAt, a => a.MapFrom(p => p.LastDownloadedAt))
                .ForMember(p => p.DownloadCount, a => a.MapFrom(p => p.DownloadCount))
                ;
        }
    }
}
