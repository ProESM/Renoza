using AutoMapper;
using Renoza.Domain.Entities.Products;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для товаров и услуг
    /// </summary>
    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<Product, ProductDao>().ReverseMap();
        }
    }
}
