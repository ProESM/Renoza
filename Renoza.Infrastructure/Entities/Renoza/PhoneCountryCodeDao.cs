using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Международный телефонный код
    /// </summary>
    public class PhoneCountryCodeDao : EntityWithIdDao<int>
    {
        /// <summary>
        /// Код
        /// </summary>
        [MaxLength(8)]
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Формат телефонного номера (например: (XXX) XXX-XX-XX)
        /// </summary>
        [MaxLength(50)]
        public string PhoneFormat { get; set; } = string.Empty;
        /// <summary>
        /// Идентификатор страны
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// Страна
        /// </summary>
        public virtual CountryDao Country { get; set; } = null!;
    }
}
