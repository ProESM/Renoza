using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность типа единицы измерения
    /// </summary>
    public class MeasurementUnitTypeDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Уникальный код
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к единицам измерения этого типа
        /// </summary>
        public virtual ICollection<MeasurementUnitDao> MeasurementUnits { get; set; } = new List<MeasurementUnitDao>();
    }
}
