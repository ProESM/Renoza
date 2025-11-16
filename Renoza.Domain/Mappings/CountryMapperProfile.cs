using Renoza.Domain.Entities.Countries;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CountryMapperProfile : AutoMapper.Profile
    {
        public CountryMapperProfile()
        {
            CreateMap<CountryDao, Country>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.Code, a => a.MapFrom(p => p.Code))
                .ForMember(p => p.Name, a => a.MapFrom(p => p.Name))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => DateTime.SpecifyKind(p.CreatedAt, DateTimeKind.Utc)))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => DateTime.SpecifyKind(p.UpdatedAt, DateTimeKind.Utc)))
                ;

            CreateMap<PhoneCountryCodeDao, PhoneCountryCode>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.Code, a => a.MapFrom(p => p.Code))
                .ForMember(p => p.PhoneFormat, a => a.MapFrom(p => p.PhoneFormat))
                .ForMember(p => p.CountryId, a => a.MapFrom(p => p.CountryId))
                .ForMember(p => p.Country, a => a.MapFrom(p => p.Country))
                ;
        }
    }
}
