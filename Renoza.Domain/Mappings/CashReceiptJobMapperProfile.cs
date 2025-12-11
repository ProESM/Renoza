using Renoza.Domain.Entities.CashReceipts;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CashReceiptJobMapperProfile : AutoMapper.Profile
    {
        public CashReceiptJobMapperProfile()
        {
            CreateMap<CashReceiptJobDao, CashReceiptJob>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.CustomerId, a => a.MapFrom(p => p.CustomerId))
                .ForMember(p => p.CreatedBy, a => a.MapFrom(p => p.CreatedBy))
                .ForMember(p => p.OrderId, a => a.MapFrom(p => p.OrderId))
                .ForMember(p => p.StatusId, a => a.MapFrom(p => p.StatusId))
                .ForMember(p => p.StatusName, a => a.MapFrom(p => p.Status.Name))
                .ForMember(p => p.StatusComment, a => a.MapFrom(p => p.StatusComment))
                .ForMember(p => p.QrSource, a => a.MapFrom(p => p.QrSource))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => p.UpdatedAt))
                .ForMember(p => p.CompletedAt, a => a.MapFrom(p => p.CompletedAt))
                ;
        }
    }
}
