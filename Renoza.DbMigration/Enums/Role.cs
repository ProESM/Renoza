using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Роль
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Администратор системы
        /// </summary>
        [RoleDetails("00000000-0000-0000-0000-000000000001", "Администратор", "Администратор системы", true, true)]
        Administrator,
        /// <summary>
        /// Заказчик ремонтных работ
        /// </summary>
        [RoleDetails("00000000-0000-0000-0000-000000000002", "Заказчик", "Заказчик ремонтных работ", false, true)]
        Customer,
        /// <summary>
        /// Работник/Исполнитель ремонтных работ
        /// </summary>
        [RoleDetails("00000000-0000-0000-0000-000000000003", "Работник", "Работник/Исполнитель ремонтных работ", false, true)]
        Worker,
        /// <summary>
        /// Технический надзор
        /// </summary>
        [RoleDetails("00000000-0000-0000-0000-000000000004", "Технический надзор", "Технический надзор", false, true)]
        TechnicalSupervisor
    }
}
