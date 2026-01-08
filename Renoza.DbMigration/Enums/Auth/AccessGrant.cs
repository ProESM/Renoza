using Renoza.DbMigration.Attributes.Auth;

namespace Renoza.DbMigration.Enums.Auth
{
    /// <summary>
    /// Гранты доступа общесистемные
    /// </summary>
    public enum AccessGrant
    {
        #region Administrator

        /// <summary>
        /// Полный административный доступ
        /// </summary>
        [AccessGrantDetails("00000000-0000-0000-0003-000000000001", "admin_full_access", "Полный административный доступ")]
        AdminFullAccess,

        #endregion

        #region Customer

        /// <summary>
        /// Доступ заказчика
        /// </summary>
        [AccessGrantDetails("00000000-0000-0000-0003-000000000010", "customer_access", "Базовый доступ заказчика")]
        CustomerAccess,

        #endregion

        #region Worker

        /// <summary>
        /// Доступ работника
        /// </summary>
        [AccessGrantDetails("00000000-0000-0000-0003-000000000020", "worker_access", "Базовый доступ работника")]
        WorkerAccess,

        #endregion

        #region TechnicalSupervisor

        /// <summary>
        /// Доступ технического надзора
        /// </summary>
        [AccessGrantDetails("00000000-0000-0000-0003-000000000030", "technical_supervisor_access", "Базовый доступ технического надзора")]
        TechnicalSupervisorAccess

        #endregion
    }
}
