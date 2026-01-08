using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.ProductCategories
{
    /// <summary>
    /// Запрос на обновление категории товаров/услуг
    /// </summary>
    public class UpdateProductCategoryRequest
    {
        /// <summary>
        /// Новое название категории (если null, не изменяется)
        /// </summary>
        [StringLength(256, ErrorMessage = "Название категории не может превышать 256 символов")]
        public string? Name { get; set; }

        /// <summary>
        /// Новое описание категории (если null, не изменяется)
        /// </summary>
        [StringLength(2000, ErrorMessage = "Описание категории не может превышать 2000 символов")]
        public string? Description { get; set; }

        /// <summary>
        /// Новый порядок сортировки (если null, не изменяется)
        /// </summary>
        public int? Order { get; set; }
    }
}
