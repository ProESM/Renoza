using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.ProfileProductPrices
{
    /// <summary>
    /// Запрос на обновление цены для профиля на товар/услугу
    /// </summary>
    public class UpdatePriceRequest
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
        /// Текущая дата начала действия цены
        /// </summary>
        [Required(ErrorMessage = "Дата начала действия цены обязательна")]
        public DateTime OldStartDate { get; set; }

        /// <summary>
        /// Новая цена (если null, не изменяется)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
        public decimal? NewPrice { get; set; }

        /// <summary>
        /// Новая валюта (если null, не изменяется)
        /// </summary>
        public Guid? NewCurrencyId { get; set; }

        /// <summary>
        /// Новая дата окончания (если null, не изменяется)
        /// </summary>
        public DateTime? NewEndDate { get; set; }
    }
}
