using AutoMapper;
using Renoza.Domain.Entities.CompanyMembers;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CompanyMemberMapperProfile : Profile
    {
        public CompanyMemberMapperProfile()
        {
            CreateMap<CompanyMemberDao, CompanyMember>().ReverseMap();
        }
    }
}
