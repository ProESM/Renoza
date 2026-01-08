namespace Renoza.Domain.Entities.Products
{
    /// <summary>
    /// Справочник валют
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Идентификатор валюты
        /// </summary>
        public Guid Id { get; set; }

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
    }
}
