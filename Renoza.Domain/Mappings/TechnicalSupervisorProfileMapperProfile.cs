using AutoMapper;
using Renoza.Domain.Entities.Profiles;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class TechnicalSupervisorProfileMapperProfile : Profile
    {
        public TechnicalSupervisorProfileMapperProfile()
        {
            CreateMap<TechnicalSupervisorProfileDao, TechnicalSupervisorProfile>().ReverseMap();
        }
    }
}
