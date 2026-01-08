namespace Renoza.Domain.Entities.Products
{
    /// <summary>
    /// Категория товаров и услуг (иерархическая структура)
    /// </summary>
    public class ProductCategory
    {
        /// <summary>
        /// Идентификатор категории
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание категории
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID родительской категории (null для корневых категорий)
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Активна ли категория
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Порядок сортировки внутри родительской категории
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
