namespace Renoza.DbMigration.Attributes
{
    /// <summary>
    /// Атрибут детализации формата документа
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DocumentFormatDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public short Id { get; }

        /// <summary>
        /// Уникальный код
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        public string? DisplayName { get; }

        /// <summary>
        /// Расширение файла
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Атрибут детализации формата документа
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="code">Уникальный код</param>
        /// <param name="name">Наименование</param>
        /// <param name="displayName">Отображаемое наименование</param>
        /// <param name="fileExtension">Расширение файла</param>
        /// <param name="isActive">Признак активности</param>
        public DocumentFormatDetailsAttribute(short id, string code, string name, string? displayName, string fileExtension, bool isActive)
        {
            Id = id;
            Code = code;
            Name = name;
            DisplayName = displayName;
            FileExtension = fileExtension;
            IsActive = isActive;
        }
    }
}
