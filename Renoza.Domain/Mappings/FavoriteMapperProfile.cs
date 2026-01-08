using AutoMapper;
using Renoza.Domain.Entities.Favorites;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для сущности Favorite
    /// </summary>
    public class FavoriteMapperProfile : Profile
    {
        public FavoriteMapperProfile()
        {
            CreateMap<FavoriteDao, Favorite>().ReverseMap();
        }
    }
}
