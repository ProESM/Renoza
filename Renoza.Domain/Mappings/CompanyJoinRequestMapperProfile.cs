using AutoMapper;
using Renoza.Domain.Entities.CompanyMembers;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для сущности CompanyJoinRequest
    /// </summary>
    public class CompanyJoinRequestMapperProfile : Profile
    {
        public CompanyJoinRequestMapperProfile()
        {
            CreateMap<CompanyJoinRequestDao, CompanyJoinRequest>().ReverseMap();
        }
    }
}
