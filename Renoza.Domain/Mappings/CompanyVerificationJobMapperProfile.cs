using AutoMapper;
using Renoza.Domain.Entities.CompanyVerification;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CompanyVerificationJobMapperProfile : Profile
    {
        public CompanyVerificationJobMapperProfile()
        {
            CreateMap<CompanyVerificationJobDao, CompanyVerificationJob>().ReverseMap();
        }
    }
}
