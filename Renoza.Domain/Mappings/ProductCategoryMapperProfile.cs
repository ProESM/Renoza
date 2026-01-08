using AutoMapper;
using Renoza.Domain.Entities.Products;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для категорий товаров и услуг
    /// </summary>
    public class ProductCategoryMapperProfile : Profile
    {
        public ProductCategoryMapperProfile()
        {
            CreateMap<ProductCategory, ProductCategoryDao>().ReverseMap();
        }
    }
}
