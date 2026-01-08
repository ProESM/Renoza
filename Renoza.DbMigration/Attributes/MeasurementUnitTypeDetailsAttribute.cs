namespace Renoza.DbMigration.Attributes
{
    /// <summary>
    /// Атрибут для описания типа единицы измерения
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MeasurementUnitTypeDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

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
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Атрибут для описания типа единицы измерения
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="code">Уникальный код</param>
        /// <param name="name">Наименование</param>
        /// <param name="description">Отображаемое наименование</param>
        /// <param name="isActive">Признак активности</param>
        public MeasurementUnitTypeDetailsAttribute(string id, string code, string name, string description, bool isActive = true)
        {
            Id = new Guid(id);
            Code = code;
            Name = name;
            Description = description;
            IsActive = isActive;
        }
    }
}
