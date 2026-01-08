using AutoMapper;
using Renoza.Domain.Entities.Products;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    /// <summary>
    /// Профиль маппинга для единиц измерения
    /// </summary>
    public class MeasurementUnitMapperProfile : Profile
    {
        public MeasurementUnitMapperProfile()
        {
            CreateMap<MeasurementUnit, MeasurementUnitDao>().ReverseMap();
        }
    }
}
