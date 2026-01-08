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
        Administrator,

        /// <summary>
        /// Заказчик ремонтных работ
        /// </summary>
        Customer,

        /// <summary>
        /// Работник/Исполнитель ремонтных работ
        /// </summary>
        Worker,

        /// <summary>
        /// Технический надзор
        /// </summary>
        TechnicalSupervisor
    }
}
