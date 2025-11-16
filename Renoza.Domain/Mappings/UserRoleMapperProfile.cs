using Renoza.Domain.Entities.Roles;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class UserRoleMapperProfile : AutoMapper.Profile
    {
        public UserRoleMapperProfile()
        {
            CreateMap<UserRoleDao, UserRole>()
                .ForMember(p => p.UserId, a => a.MapFrom(p => p.UserId))
                .ForMember(p => p.RoleId, a => a.MapFrom(p => p.RoleId))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => p.UpdatedAt))
                ;
        }
    }
}
