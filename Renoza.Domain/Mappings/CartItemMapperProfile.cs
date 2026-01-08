using AutoMapper;
using Renoza.Domain.Entities.Cart;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для элементов корзины
    /// </summary>
    public class CartItemMapperProfile : Profile
    {
        public CartItemMapperProfile()
        {
            CreateMap<CartItem, CartItemDao>().ReverseMap();
        }
    }
}
