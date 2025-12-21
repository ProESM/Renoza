namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Статус документа
    /// </summary>
    public enum DocumentStatus : short
    {
        /// <summary>
        /// Черновик
        /// </summary>
        Draft = 1,

        /// <summary>
        /// Сгенерирован
        /// </summary>
        Generated = 2,

        /// <summary>
        /// Скачан
        /// </summary>
        Downloaded = 3,

        /// <summary>
        /// Архивирован
        /// </summary>
        Archived = 4,

        /// <summary>
        /// Удалён
        /// </summary>
        Deleted = 5
    }
}
