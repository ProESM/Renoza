using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Cart
{
    /// <summary>
    /// Модель запроса для обновления количества товара в корзине
    /// </summary>
    public class UpdateQuantityRequest
    {
        /// <summary>
        /// Новое количество
        /// </summary>
        [Required(ErrorMessage = "Количество обязательно")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Количество должно быть больше нуля")]
        public decimal Quantity { get; set; }
    }
}
