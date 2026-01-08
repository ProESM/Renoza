namespace Renoza.Domain.Entities.Products
{
    /// <summary>
    /// Единица измерения товаров и услуг
    /// </summary>
    public class MeasurementUnit
    {
        /// <summary>
        /// Идентификатор единицы измерения
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор типа единицы измерения
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Короткое обозначение (шт, м², м³, кг, час и т.д.)
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Полное название (штука, квадратный метр, кубический метр, килограмм, час)
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание единицы измерения
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Коэффициент относительно базовой единицы.
        /// Это сколько базовых единиц содержится в одной текущей
        /// </summary>
        public decimal Multiplier { get; set; }

        /// <summary>
        /// Признак базовой единицы измерения в рамках типа
        /// </summary>
        public bool IsBase { get; set; } = false;

        /// <summary>
        /// Активна ли единица измерения
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
