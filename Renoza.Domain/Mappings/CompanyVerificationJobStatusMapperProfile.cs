using AutoMapper;
using Renoza.Domain.Entities.CompanyVerification;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CompanyVerificationJobStatusMapperProfile : Profile
    {
        public CompanyVerificationJobStatusMapperProfile()
        {
            CreateMap<CompanyVerificationJobStatusDao, CompanyVerificationJobStatus>().ReverseMap();
        }
    }
}
