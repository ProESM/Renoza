using Renoza.Domain.Entities.Users;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class UserMapperProfile : AutoMapper.Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserDao, User>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.Name, a => a.MapFrom(p => p.Name))
                ;
        }
    }
}
