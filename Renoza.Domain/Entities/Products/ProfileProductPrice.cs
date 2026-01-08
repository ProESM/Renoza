namespace Renoza.Domain.Entities.Products
{
    /// <summary>
    /// Базовая цена профиля (исполнителя) на товар/услугу
    /// Составной PK: (ProfileId, ProductId, StartDate)
    /// </summary>
    public class ProfileProductPrice
    {
        /// <summary>
        /// ID профиля исполнителя (Profile - базовая таблица для TPT)
        /// Часть составного первичного ключа
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// ID товара/услуги
        /// Часть составного первичного ключа
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Дата начала действия цены
        /// Часть составного первичного ключа
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания действия цены (для бессрочных = DateTime.MaxValue)
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// ID валюты (базовая валюта для данной цены)
        /// </summary>
        public Guid CurrencyId { get; set; }

        /// <summary>
        /// Цена за единицу товара/услуги в базовой валюте
        /// </summary>
        public decimal Price { get; set; }
    }
}
