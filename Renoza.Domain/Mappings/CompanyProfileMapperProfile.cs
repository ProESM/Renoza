using AutoMapper;
using Renoza.Domain.Entities.CompanyProfiles;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CompanyProfileMapperProfile : Profile
    {
        public CompanyProfileMapperProfile()
        {
            CreateMap<CompanyProfileDao, CompanyProfile>().ReverseMap();
        }
    }
}
