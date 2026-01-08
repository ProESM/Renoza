using Renoza.DbMigration.Attributes.Company;

namespace Renoza.DbMigration.Enums.Company
{
    /// <summary>
    /// Роли участников компании
    /// </summary>
    public enum CompanyMemberRole
    {
        /// <summary>
        /// Владелец - полный контроль над компанией
        /// </summary>
        [CompanyMemberRoleDetails("10000000-0000-0000-0004-000000000001", "owner", "Владелец", "Владелец компании", true, true)]
        Owner,

        /// <summary>
        /// Менеджер - управление заказами и сотрудниками
        /// </summary>
        [CompanyMemberRoleDetails("10000000-0000-0000-0004-000000000002", "manager", "Менеджер", "Менеджер компании", true, true)]
        Manager,

        /// <summary>
        /// Сотрудник - базовые права на работу
        /// </summary>
        [CompanyMemberRoleDetails("10000000-0000-0000-0004-000000000003", "employee", "Сотрудник", "Сотрудник компании", true, true)]
        Employee
    }
}
