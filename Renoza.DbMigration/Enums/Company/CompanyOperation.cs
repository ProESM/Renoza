using Renoza.DbMigration.Attributes.Company;

namespace Renoza.DbMigration.Enums.Company
{
    /// <summary>
    /// Операции компании
    /// </summary>
    public enum CompanyOperation
    {
        /// <summary>
        /// Создание
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000001", "create", "Создание")]
        Create,

        /// <summary>
        /// Чтение
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000002", "read", "Чтение")]
        Read,

        /// <summary>
        /// Изменение
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000003", "update", "Изменение")]
        Update,

        /// <summary>
        /// Удаление
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000004", "delete", "Удаление")]
        Delete,

        /// <summary>
        /// Выполнение
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000005", "execute", "Выполнение")]
        Execute,

        /// <summary>
        /// Утверждение
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000006", "approve", "Утверждение")]
        Approve,

        /// <summary>
        /// Отклонение
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000007", "reject", "Отклонение")]
        Reject,

        /// <summary>
        /// Экспорт
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000008", "export", "Экспорт")]
        Export,

        /// <summary>
        /// Импорт
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000009", "import", "Импорт")]
        Import,

        /// <summary>
        /// Приглашение
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000010", "invite", "Приглашение")]
        Invite,

        /// <summary>
        /// Удаление участника
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000011", "remove", "Удаление участника")]
        Remove,

        /// <summary>
        /// Назначение
        /// </summary>
        [CompanyOperationDetails("10000000-0000-0000-0001-000000000012", "assign", "Назначение")]
        Assign
    }
}
