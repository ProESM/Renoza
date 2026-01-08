using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность единицы измерения
    /// </summary>
    public class MeasurementUnitDao : EntityWithIdDao<Guid>
    {
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
        /// Коэффициент относительно базовой.
        /// Это сколько базовых единиц содержится в одной текущей
        /// </summary>
        public decimal Multiplier { get; set; }

        /// <summary>
        /// Признак базовой единицы измерения в рамках типа
        /// </summary>
        public bool IsBase { get; set; } = false;

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к типу единицы измерения
        /// </summary>
        public virtual MeasurementUnitTypeDao Type { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к товарам с этой единицей измерения
        /// </summary>
        public virtual ICollection<ProductDao> Products { get; set; } = new List<ProductDao>();
    }
}
