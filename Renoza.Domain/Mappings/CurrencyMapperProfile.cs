using AutoMapper;
using Renoza.Domain.Entities.Products;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для валют
    /// </summary>
    public class CurrencyMapperProfile : Profile
    {
        public CurrencyMapperProfile()
        {
            CreateMap<Currency, CurrencyDao>().ReverseMap();
        }
    }
}
