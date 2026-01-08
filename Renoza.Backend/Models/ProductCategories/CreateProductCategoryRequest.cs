using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.ProductCategories
{
    /// <summary>
    /// Запрос на создание категории товаров/услуг
    /// </summary>
    public class CreateProductCategoryRequest
    {
        /// <summary>
        /// Название категории
        /// </summary>
        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(256, ErrorMessage = "Название категории не может превышать 256 символов")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание категории
        /// </summary>
        [StringLength(2000, ErrorMessage = "Описание категории не может превышать 2000 символов")]
        public string? Description { get; set; }

        /// <summary>
        /// ID родительской категории (null для корневой категории)
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Порядок сортировки внутри родительской категории
        /// </summary>
        public int Order { get; set; } = 0;
    }
}
