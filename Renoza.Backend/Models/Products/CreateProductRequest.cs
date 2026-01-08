using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Products
{
    /// <summary>
    /// Запрос на создание товара/услуги
    /// </summary>
    public class CreateProductRequest
    {
        /// <summary>
        /// ID категории товара/услуги
        /// </summary>
        [Required(ErrorMessage = "ID категории обязателен")]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Название товара/услуги
        /// </summary>
        [Required(ErrorMessage = "Название товара/услуги обязательно")]
        [StringLength(500, ErrorMessage = "Название товара/услуги не может превышать 500 символов")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание товара/услуги
        /// </summary>
        [StringLength(2000, ErrorMessage = "Описание товара/услуги не может превышать 2000 символов")]
        public string? Description { get; set; }

        /// <summary>
        /// ID единицы измерения
        /// </summary>
        [Required(ErrorMessage = "ID единицы измерения обязателен")]
        public Guid MeasurementUnitId { get; set; }
    }
}
