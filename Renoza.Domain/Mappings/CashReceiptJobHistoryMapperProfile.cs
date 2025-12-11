using Renoza.Domain.Entities.CashReceipts;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class CashReceiptJobHistoryMapperProfile : AutoMapper.Profile
    {
        public CashReceiptJobHistoryMapperProfile()
        {
            CreateMap<CashReceiptJobHistoryDao, CashReceiptJobHistory>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.JobId, a => a.MapFrom(p => p.JobId))
                .ForMember(p => p.StatusId, a => a.MapFrom(p => p.StatusId))
                .ForMember(p => p.StatusName, a => a.MapFrom(p => p.Status.Name))
                .ForMember(p => p.Comment, a => a.MapFrom(p => p.Comment))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                ;
        }
    }
}
