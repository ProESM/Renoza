using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность категории товаров и услуг
    /// </summary>
    public class ProductCategoryDao : EntityWithIdDao<Guid>
    {
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

        /// <summary>
        /// Навигационное свойство к родительской категории
        /// </summary>
        public virtual ProductCategoryDao? Parent { get; set; }

        /// <summary>
        /// Навигационное свойство к дочерним категориям
        /// </summary>
        public virtual ICollection<ProductCategoryDao> Children { get; set; } = new List<ProductCategoryDao>();

        /// <summary>
        /// Навигационное свойство к товарам в категории
        /// </summary>
        public virtual ICollection<ProductDao> Products { get; set; } = new List<ProductDao>();
    }
}
