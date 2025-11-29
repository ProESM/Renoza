using Renoza.Domain.Entities.Verifications;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Mappings
{
    public class EmailVerificationMapperProfile : AutoMapper.Profile
    {
        public EmailVerificationMapperProfile()
        {
            CreateMap<EmailVerificationDao, EmailVerification>()
                .ForMember(p => p.Id, a => a.MapFrom(p => p.Id))
                .ForMember(p => p.UserId, a => a.MapFrom(p => p.UserId))
                .ForMember(p => p.Email, a => a.MapFrom(p => p.Email))
                .ForMember(p => p.VerificationCode, a => a.MapFrom(p => p.VerificationCode))
                .ForMember(p => p.IsVerified, a => a.MapFrom(p => p.IsVerified))
                .ForMember(p => p.ExpiresAt, a => a.MapFrom(p => p.ExpiresAt))
                .ForMember(p => p.VerifiedAt, a => a.MapFrom(p => p.VerifiedAt))
                .ForMember(p => p.CreatedAt, a => a.MapFrom(p => p.CreatedAt))
                .ForMember(p => p.UpdatedAt, a => a.MapFrom(p => p.UpdatedAt))
                ;
        }
    }
}
