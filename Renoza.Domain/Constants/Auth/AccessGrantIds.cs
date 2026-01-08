namespace Renoza.Domain.Constants.Auth
{
    /// <summary>
    /// Идентификаторы грантов доступа auth схемы
    /// </summary>
    public static class AccessGrantIds
    {
        /// <summary>
        /// Полный административный доступ
        /// </summary>
        public static readonly Guid AdminFullAccess = new Guid("00000000-0000-0000-0003-000000000001");

        /// <summary>
        /// Доступ заказчика
        /// </summary>
        public static readonly Guid CustomerAccess = new Guid("00000000-0000-0000-0003-000000000010");

        /// <summary>
        /// Доступ работника
        /// </summary>
        public static readonly Guid WorkerAccess = new Guid("00000000-0000-0000-0003-000000000020");

        /// <summary>
        /// Доступ технического надзора
        /// </summary>
        public static readonly Guid TechnicalSupervisorAccess = new Guid("00000000-0000-0000-0003-000000000030");
    }
}
