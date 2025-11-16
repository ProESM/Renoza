namespace Renoza.Domain.Constants
{
    /// <summary>
    /// Идентификаторы ролей (Guid)
    /// </summary>
    public static class RoleIds
    {
        /// <summary>
        /// Идентификатор роли Администратор
        /// </summary>
        public static readonly Guid Administrator = new Guid("00000000-0000-0000-0000-000000000001");

        /// <summary>
        /// Идентификатор роли Заказчик
        /// </summary>
        public static readonly Guid Customer = new Guid("00000000-0000-0000-0000-000000000002");

        /// <summary>
        /// Идентификатор роли Работник
        /// </summary>
        public static readonly Guid Worker = new Guid("00000000-0000-0000-0000-000000000003");

        /// <summary>
        /// Идентификатор роли Технический надзор
        /// </summary>
        public static readonly Guid TechnicalSupervisor = new Guid("00000000-0000-0000-0000-000000000004");
    }
}
