using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Страна
    /// </summary>
    public class CountryDao : EntityWithIdDao<int>
    {
        /// <summary>
        /// Код
        /// </summary>
        [MaxLength(3)]
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Наименование
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<PhoneCountryCodeDao> PhoneCountryCodes { get; set; } = null!;
    }
}
