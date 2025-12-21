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
        /// Наименование
        /// </summary>
        public string Name { get; }

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
        /// <param name="name">Наименование</param>
        /// <param name="fileExtension">Расширение файла</param>
        /// <param name="isActive">Признак активности</param>
        public DocumentFormatDetailsAttribute(short id, string name, string fileExtension, bool isActive)
        {
            Id = id;
            Name = name;
            FileExtension = fileExtension;
            IsActive = isActive;
        }
    }
}
