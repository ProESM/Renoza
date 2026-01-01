using AutoMapper;
using Renoza.Domain.Entities.CompanyVerification;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CompanyVerificationMapperProfile : Profile
    {
        public CompanyVerificationMapperProfile()
        {
            CreateMap<CompanyVerificationDao, CompanyVerification>().ReverseMap();
        }
    }
}
