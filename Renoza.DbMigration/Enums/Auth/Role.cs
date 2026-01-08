using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums.Auth
{
    /// <summary>
    /// Роль
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Администратор системы
        /// </summary>
        [RoleDetails("00000000-0000-0000-0000-000000000001", "administrator", "Администратор", "Администратор системы", true, true)]
        Administrator,
        /// <summary>
        /// Заказчик ремонтных работ
        /// </summary>
        [RoleDetails("00000000-0000-0000-0000-000000000002", "customer", "Заказчик", "Заказчик ремонтных работ", false, true)]
        Customer,
        /// <summary>
        /// Работник/Исполнитель ремонтных работ
        /// </summary>
        [RoleDetails("00000000-0000-0000-0000-000000000003", "worker", "Работник", "Работник/Исполнитель ремонтных работ", false, true)]
        Worker,
        /// <summary>
        /// Технический надзор
        /// </summary>
        [RoleDetails("00000000-0000-0000-0000-000000000004", "technical_supervisor", "Технический надзор", "Технический надзор", false, true)]
        TechnicalSupervisor
    }
}
