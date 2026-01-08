using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность валюты
    /// </summary>
    public class CurrencyDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Код валюты (RUB, USD, EUR и т.д.)
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Название валюты (Российский рубль, Доллар США, Евро)
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Символ валюты (₽, $, €)
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// Активна ли валюта
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к базовым ценам в этой валюте
        /// </summary>
        public virtual ICollection<ProfileProductPriceDao> ProfileProductPrices { get; set; } = new List<ProfileProductPriceDao>();

        /// <summary>
        /// Навигационное свойство к переопределенным ценам в этой валюте
        /// </summary>
        public virtual ICollection<ProfileProductOverridePriceDao> ProfileProductOverridePrices { get; set; } = new List<ProfileProductOverridePriceDao>();
    }
}
