namespace Renoza.Domain.Entities.Products
{
    /// <summary>
    /// Товар или услуга в каталоге
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Идентификатор товара/услуги
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID категории
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
    }
}
