using System.ComponentModel.DataAnnotations;

namespace Renoza.Domain.Options
{
    public class JwtSettings
    {
        [Required]
        public string SecretKey { get; set; }
        [Required]
        public string Issuer { get; set; }
        [Required]
        public string Audience { get; set; }
        [Range(1, 1440)]
        public int? ExpirationMinutes { get; set; } = 60;
    }
}
