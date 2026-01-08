namespace Renoza.Domain.Constants.Auth
{
    /// <summary>
    /// Идентификаторы ресурсов auth схемы
    /// </summary>
    public static class ResourceIds
    {
        /// <summary>
        /// Пользователи
        /// </summary>
        public static readonly Guid Users = new Guid("00000000-0000-0000-0002-000000000001");

        /// <summary>
        /// Управление пользователями
        /// </summary>
        public static readonly Guid UsersManage = new Guid("00000000-0000-0000-0002-000000000002");

        /// <summary>
        /// Роли
        /// </summary>
        public static readonly Guid Roles = new Guid("00000000-0000-0000-0002-000000000010");

        /// <summary>
        /// Управление ролями
        /// </summary>
        public static readonly Guid RolesManage = new Guid("00000000-0000-0000-0002-000000000011");

        /// <summary>
        /// Система
        /// </summary>
        public static readonly Guid System = new Guid("00000000-0000-0000-0002-000000000020");

        /// <summary>
        /// Настройки системы
        /// </summary>
        public static readonly Guid SystemSettings = new Guid("00000000-0000-0000-0002-000000000021");

        /// <summary>
        /// Логи системы
        /// </summary>
        public static readonly Guid SystemLogs = new Guid("00000000-0000-0000-0002-000000000022");
    }
}
