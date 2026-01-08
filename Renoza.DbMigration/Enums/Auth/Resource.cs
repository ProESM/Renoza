using Renoza.DbMigration.Attributes.Auth;

namespace Renoza.DbMigration.Enums.Auth
{
    /// <summary>
    /// Ресурсы общесистемные
    /// </summary>
    public enum Resource
    {
        #region Пользователи

        /// <summary>
        /// Пользователи (корневой ресурс)
        /// </summary>
        [ResourceDetails("00000000-0000-0000-0002-000000000001", "users", "Пользователи")]
        Users,

        /// <summary>
        /// Управление пользователями
        /// </summary>
        [ResourceDetails("00000000-0000-0000-0002-000000000002", "users.manage", "Управление пользователями", "00000000-0000-0000-0002-000000000001")]
        UsersManage,

        #endregion

        #region Роли

        /// <summary>
        /// Роли (корневой ресурс)
        /// </summary>
        [ResourceDetails("00000000-0000-0000-0002-000000000010", "roles", "Роли")]
        Roles,

        /// <summary>
        /// Управление ролями
        /// </summary>
        [ResourceDetails("00000000-0000-0000-0002-000000000011", "roles.manage", "Управление ролями", "00000000-0000-0000-0002-000000000010")]
        RolesManage,

        #endregion

        #region Система

        /// <summary>
        /// Система (корневой ресурс)
        /// </summary>
        [ResourceDetails("00000000-0000-0000-0002-000000000020", "system", "Система")]
        System,

        /// <summary>
        /// Настройки системы
        /// </summary>
        [ResourceDetails("00000000-0000-0000-0002-000000000021", "system.settings", "Настройки системы", "00000000-0000-0000-0002-000000000020")]
        SystemSettings,

        /// <summary>
        /// Логи системы
        /// </summary>
        [ResourceDetails("00000000-0000-0000-0002-000000000022", "system.logs", "Логи системы", "00000000-0000-0000-0002-000000000020")]
        SystemLogs

        #endregion
    }
}
