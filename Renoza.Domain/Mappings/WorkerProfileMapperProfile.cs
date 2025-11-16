using Renoza.Domain.Entities.Profiles;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class WorkerProfileMapperProfile : AutoMapper.Profile
    {
        public WorkerProfileMapperProfile()
        {
            CreateMap<WorkerProfileDao, WorkerProfile>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.UserId, a => a.MapFrom(p => p.UserId))
                .ForMember(p => p.Specialization, a => a.MapFrom(p => p.Specialization))
                .ForMember(p => p.TeamSize, a => a.MapFrom(p => p.TeamSize))
                .ForMember(p => p.Certifications, a => a.MapFrom(p => p.Certifications))
                .ForMember(p => p.ProfessionalStartDate, a => a.MapFrom(p => p.ProfessionalStartDate))
                .ForMember(p => p.IsAvailable, a => a.MapFrom(p => p.IsAvailable))
                .ForMember(p => p.Rating, a => a.MapFrom(p => p.Rating))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => p.UpdatedAt))
                ;
        }
    }
}
