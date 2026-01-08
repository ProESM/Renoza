namespace Renoza.Domain.Entities.Products
{
    /// <summary>
    /// Переопределение цены в альтернативной валюте
    /// Составной PK: (ProfileId, ProductId, StartDate, CurrencyId)
    /// FK на ProfileProductPrice по (ProfileId, ProductId, StartDate)
    /// EndDate наследуется из базовой записи ProfileProductPrice
    /// </summary>
    public class ProfileProductOverridePrice
    {
        /// <summary>
        /// ID профиля исполнителя
        /// Часть составного первичного ключа и FK
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// ID товара/услуги
        /// Часть составного первичного ключа и FK
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Дата начала действия (берется из базовой цены)
        /// Часть составного первичного ключа и FK
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// ID альтернативной валюты
        /// Часть составного первичного ключа
        /// </summary>
        public Guid CurrencyId { get; set; }

        /// <summary>
        /// Цена в альтернативной валюте
        /// </summary>
        public decimal Price { get; set; }
    }
}
