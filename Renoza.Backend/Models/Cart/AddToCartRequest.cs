using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Cart
{
    /// <summary>
    /// Модель запроса для добавления товара в корзину
    /// </summary>
    public class AddToCartRequest
    {
        /// <summary>
        /// ID профиля исполнителя
        /// </summary>
        [Required(ErrorMessage = "ID профиля обязателен")]
        public Guid ProfileId { get; set; }

        /// <summary>
        /// ID товара/услуги
        /// </summary>
        [Required(ErrorMessage = "ID товара обязателен")]
        public Guid ProductId { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        [Required(ErrorMessage = "Количество обязательно")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Количество должно быть больше нуля")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Примечания к заказу
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Примечание не может быть длиннее 1000 символов")]
        public string? Notes { get; set; }
    }
}
