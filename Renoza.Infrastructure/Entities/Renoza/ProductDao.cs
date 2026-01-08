using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность товара/услуги
    /// </summary>
    public class ProductDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// ID категории товара/услуги
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Название товара/услуги
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание товара/услуги
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID единицы измерения
        /// </summary>
        public Guid MeasurementUnitId { get; set; }

        /// <summary>
        /// Активен ли товар/услуга
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к категории
        /// </summary>
        public virtual ProductCategoryDao Category { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к единице измерения
        /// </summary>
        public virtual MeasurementUnitDao MeasurementUnit { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к базовым ценам товара/услуги
        /// </summary>
        public virtual ICollection<ProfileProductPriceDao> ProfileProductPrices { get; set; } = new List<ProfileProductPriceDao>();

        /// <summary>
        /// Навигационное свойство к элементам корзины с этим товаром
        /// </summary>
        public virtual ICollection<CartItemDao> CartItems { get; set; } = new List<CartItemDao>();
    }
}
