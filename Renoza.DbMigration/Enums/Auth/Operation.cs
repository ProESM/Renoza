using Renoza.DbMigration.Attributes.Auth;

namespace Renoza.DbMigration.Enums.Auth
{
    /// <summary>
    /// Операции общесистемные
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// Создание
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000001", "create", "Создание")]
        Create,

        /// <summary>
        /// Чтение
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000002", "read", "Чтение")]
        Read,

        /// <summary>
        /// Изменение
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000003", "update", "Изменение")]
        Update,

        /// <summary>
        /// Удаление
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000004", "delete", "Удаление")]
        Delete,

        /// <summary>
        /// Выполнение
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000005", "execute", "Выполнение")]
        Execute,

        /// <summary>
        /// Утверждение
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000006", "approve", "Утверждение")]
        Approve,

        /// <summary>
        /// Отклонение
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000007", "reject", "Отклонение")]
        Reject,

        /// <summary>
        /// Экспорт
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000008", "export", "Экспорт")]
        Export,

        /// <summary>
        /// Импорт
        /// </summary>
        [OperationDetails("00000000-0000-0000-0001-000000000009", "import", "Импорт")]
        Import
    }
}
