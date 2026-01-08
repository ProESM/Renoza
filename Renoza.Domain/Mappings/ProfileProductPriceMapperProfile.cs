using AutoMapper;
using Renoza.Domain.Entities.Products;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для базовых цен профилей на товары/услуги
    /// </summary>
    public class ProfileProductPriceMapperProfile : Profile
    {
        public ProfileProductPriceMapperProfile()
        {
            CreateMap<ProfileProductPrice, ProfileProductPriceDao>().ReverseMap();
        }
    }
}
