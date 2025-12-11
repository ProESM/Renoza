namespace Renoza.DbMigration.Attributes
{
    /// <summary>
    /// Атрибут детализации статуса запроса на загрузку чека
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CashReceiptJobStatusDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public short Id { get; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Атрибут детализации статуса запроса на загрузку чека
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Наименование</param>
        /// <param name="isActive">Признак активности</param>
        public CashReceiptJobStatusDetailsAttribute(short id, string name, bool isActive)
        {
            Id = id;
            Name = name;
            IsActive = isActive;
        }
    }
}
