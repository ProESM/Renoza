using AutoMapper;
using Renoza.Domain.Entities.Products;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для переопределённых цен в альтернативных валютах
    /// </summary>
    public class ProfileProductOverridePriceMapperProfile : Profile
    {
        public ProfileProductOverridePriceMapperProfile()
        {
            CreateMap<ProfileProductOverridePrice, ProfileProductOverridePriceDao>().ReverseMap();
        }
    }
}
