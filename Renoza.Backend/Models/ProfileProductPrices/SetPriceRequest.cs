using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.ProfileProductPrices
{
    /// <summary>
    /// Запрос на установку цены для профиля на товар/услугу
    /// </summary>
    public class SetPriceRequest
    {
        /// <summary>
        /// ID профиля (технадзор или рабочий)
        /// </summary>
        [Required(ErrorMessage = "ID профиля обязателен")]
        public Guid ProfileId { get; set; }

        /// <summary>
        /// ID товара/услуги
        /// </summary>
        [Required(ErrorMessage = "ID товара/услуги обязателен")]
        public Guid ProductId { get; set; }

        /// <summary>
        /// ID валюты
        /// </summary>
        [Required(ErrorMessage = "ID валюты обязателен")]
        public Guid CurrencyId { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
        public decimal Price { get; set; }

        /// <summary>
        /// Дата начала действия цены (null для текущей даты)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания действия цены (null для бессрочной)
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
