using Renoza.Domain.Entities.Profiles;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CustomerProfileMapperProfile : AutoMapper.Profile
    {
        public CustomerProfileMapperProfile()
        {
            CreateMap<CustomerProfileDao, CustomerProfile>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.UserId, a => a.MapFrom(p => p.UserId))
                .ForMember(p => p.CompanyName, a => a.MapFrom(p => p.CompanyName))
                .ForMember(p => p.TaxId, a => a.MapFrom(p => p.TaxId))
                .ForMember(p => p.BillingAddress, a => a.MapFrom(p => p.BillingAddress))
                .ForMember(p => p.CreditLimit, a => a.MapFrom(p => p.CreditLimit))
                .ForMember(p => p.IsActive, a => a.MapFrom(p => p.IsActive))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => p.UpdatedAt))
                ;
        }
    }
}
