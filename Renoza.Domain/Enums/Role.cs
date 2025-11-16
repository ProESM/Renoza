using System.ComponentModel;

namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Роли пользователей в системе
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Администратор системы
        /// </summary>
        [Description("Администратор")]
        Administrator,

        /// <summary>
        /// Заказчик ремонтных работ
        /// </summary>
        [Description("Заказчик")]
        Customer,

        /// <summary>
        /// Работник/Исполнитель ремонтных работ
        /// </summary>
        [Description("Работник")]
        Worker,

        /// <summary>
        /// Технический надзор
        /// </summary>
        [Description("Технический надзор")]
        TechnicalSupervisor
    }
}
