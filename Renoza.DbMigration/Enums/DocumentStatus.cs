using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Статус документа
    /// </summary>
    public enum DocumentStatus : short
    {
        /// <summary>
        /// Черновик
        /// </summary>
        [DocumentStatusDetails(1, "Draft", true)]
        Draft = 1,

        /// <summary>
        /// Сгенерирован
        /// </summary>
        [DocumentStatusDetails(2, "Generated", true)]
        Generated = 2,

        /// <summary>
        /// Скачан
        /// </summary>
        [DocumentStatusDetails(3, "Downloaded", true)]
        Downloaded = 3,

        /// <summary>
        /// Архивирован
        /// </summary>
        [DocumentStatusDetails(4, "Archived", true)]
        Archived = 4,

        /// <summary>
        /// Удалён
        /// </summary>
        [DocumentStatusDetails(5, "Deleted", true)]
        Deleted = 5
    }
}
