using Renoza.Domain.Entities.Roles;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class RoleMapperProfile : AutoMapper.Profile
    {
        public RoleMapperProfile()
        {
            CreateMap<RoleDao, Role>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.Code, a => a.MapFrom(p => p.Code))
                .ForMember(p => p.Name, a => a.MapFrom(p => p.Name))
                .ForMember(p => p.DisplayName, a => a.MapFrom(p => p.DisplayName))
                .ForMember(p => p.IsSystemRole, a => a.MapFrom(p => p.IsSystemRole))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                ;
        }
    }
}
