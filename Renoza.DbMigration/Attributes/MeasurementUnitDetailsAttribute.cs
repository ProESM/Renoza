namespace Renoza.DbMigration.Attributes
{
    /// <summary>
    /// Атрибут для описания единицы измерения
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MeasurementUnitDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор типа единицы измерения
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Уникальный код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Коэффициент относительно базовой.
        /// Это сколько базовых единиц содержится в одной текущей
        /// </summary>
        public decimal Multiplier { get; set; }

        /// <summary>
        /// Признак базовой единицы измерения в рамках типа
        /// </summary>
        public bool IsBase { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Атрибут для описания единицы измерения
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="typeId">Идентификатор типа единицы измерения</param>
        /// <param name="code">Уникальный код</param>
        /// <param name="name">Наименование</param>
        /// <param name="description">Отображаемое наименование</param>
        /// <param name="multiplier">Коэффициент относительно базовой</param>
        /// <param name="isBase">Признак базовой единицы измерения в рамках типа</param>
        /// <param name="isActive">Признак активности</param>
        public MeasurementUnitDetailsAttribute(string id, string typeId, string code, string name, string description, double multiplier, bool isBase = false, bool isActive = true)
        {
            Id = new Guid(id);
            TypeId = new Guid(typeId);
            Code = code;
            Name = name;
            Description = description;
            Multiplier = (decimal)multiplier;
            IsBase = isBase;
            IsActive = isActive;
        }
    }
}
