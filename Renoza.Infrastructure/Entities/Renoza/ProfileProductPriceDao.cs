using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность базовой цены профиля на товар/услугу с временным периодом
    /// Составной первичный ключ: (ProfileId, ProductId, StartDate)
    /// </summary>
    public class ProfileProductPriceDao : EntityDao
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
        /// Дата начала действия цены
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания действия цены (DateTime.MaxValue для бессрочной)
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// ID валюты
        /// </summary>
        public Guid CurrencyId { get; set; }

        /// <summary>
        /// Цена в указанной валюте
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Навигационное свойство к товару/услуге
        /// </summary>
        public virtual ProductDao Product { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к валюте
        /// </summary>
        public virtual CurrencyDao Currency { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к переопределённым ценам в других валютах
        /// </summary>
        public virtual ICollection<ProfileProductOverridePriceDao> OverridePrices { get; set; } = new List<ProfileProductOverridePriceDao>();
    }
}
