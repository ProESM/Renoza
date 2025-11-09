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
                .ForMember(p => p.DisplayName, a => a.MapFrom(p => p.DisplayName))
                .ForMember(p => p.Email, a => a.MapFrom(p => p.Email))
                .ForMember(p => p.IsEmailVerified, a => a.MapFrom(p => p.IsEmailVerified))
                .ForMember(p => p.PhoneNumber, a => a.MapFrom(p => p.PhoneNumber))
                .ForMember(p => p.PhoneCountryCode, a => a.MapFrom(p => p.PhoneCountryCode))
                .ForMember(p => p.IsPhoneNumberVerified, a => a.MapFrom(p => p.IsPhoneNumberVerified))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => p.UpdatedAt))
                ;
        }
    }
}
