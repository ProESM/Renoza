using AutoMapper;
using Renoza.Domain.Entities.CompanyVerification;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CompanyVerificationJobHistoryMapperProfile : Profile
    {
        public CompanyVerificationJobHistoryMapperProfile()
        {
            CreateMap<CompanyVerificationJobHistoryDao, CompanyVerificationJobHistory>().ReverseMap();
        }
    }
}
