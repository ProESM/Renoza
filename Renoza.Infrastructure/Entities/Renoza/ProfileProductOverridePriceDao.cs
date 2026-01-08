using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность переопределённой цены в альтернативной валюте
    /// Составной первичный ключ: (ProfileId, ProductId, StartDate, CurrencyId)
    /// FK на базовую цену: (ProfileId, ProductId, StartDate) -> ProfileProductPrices
    /// </summary>
    public class ProfileProductOverridePriceDao
    {
        /// <summary>
        /// ID профиля (технадзор или ремонтная бригада)
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// ID товара/услуги
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Дата начала действия цены (из базовой записи)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// ID альтернативной валюты
        /// </summary>
        public Guid CurrencyId { get; set; }

        /// <summary>
        /// Цена в альтернативной валюте
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Навигационное свойство к базовой цене
        /// </summary>
        public virtual ProfileProductPriceDao ProfileProductPrice { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к альтернативной валюте
        /// </summary>
        public virtual CurrencyDao Currency { get; set; } = null!;
    }
}
