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
        [DocumentStatusDetails(1, "draft", "Черновик", "Документ в статусе черновика", true)]
        Draft = 1,

        /// <summary>
        /// Сгенерирован
        /// </summary>
        [DocumentStatusDetails(2, "generated", "Сгенерирован", "Документ успешно сгенерирован", true)]
        Generated = 2,

        /// <summary>
        /// Скачан
        /// </summary>
        [DocumentStatusDetails(3, "downloaded", "Скачан", "Документ был скачан пользователем", true)]
        Downloaded = 3,

        /// <summary>
        /// Архивирован
        /// </summary>
        [DocumentStatusDetails(4, "archived", "Архивирован", "Документ перемещен в архив", true)]
        Archived = 4,

        /// <summary>
        /// Удалён
        /// </summary>
        [DocumentStatusDetails(5, "deleted", "Удалён", "Документ помечен как удалённый", true)]
        Deleted = 5
    }
}
