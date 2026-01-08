using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Products
{
    /// <summary>
    /// Запрос на обновление товара/услуги
    /// </summary>
    public class UpdateProductRequest
    {
        /// <summary>
        /// Новое название товара/услуги (если null, не изменяется)
        /// </summary>
        [StringLength(500, ErrorMessage = "Название товара/услуги не может превышать 500 символов")]
        public string? Name { get; set; }

        /// <summary>
        /// Новое описание товара/услуги (если null, не изменяется)
        /// </summary>
        [StringLength(2000, ErrorMessage = "Описание товара/услуги не может превышать 2000 символов")]
        public string? Description { get; set; }

        /// <summary>
        /// Новая категория товара/услуги (если null, не изменяется)
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Новая единица измерения (если null, не изменяется)
        /// </summary>
        public Guid? MeasurementUnitId { get; set; }
    }
}
