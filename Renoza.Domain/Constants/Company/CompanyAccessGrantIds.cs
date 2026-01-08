namespace Renoza.Domain.Constants.Company
{
    /// <summary>
    /// Идентификаторы грантов доступа компании
    /// </summary>
    public static class CompanyAccessGrantIds
    {
        #region Owner (Владелец)

        /// <summary>
        /// Полный доступ владельца компании
        /// </summary>
        public static readonly Guid OwnerFull = new Guid("10000000-0000-0000-0003-000000000001");

        #endregion

        #region Manager (Менеджер)

        /// <summary>
        /// Управление заказами
        /// </summary>
        public static readonly Guid ManagerOrders = new Guid("10000000-0000-0000-0003-000000000010");

        /// <summary>
        /// Управление участниками
        /// </summary>
        public static readonly Guid ManagerMembers = new Guid("10000000-0000-0000-0003-000000000011");

        /// <summary>
        /// Просмотр финансов
        /// </summary>
        public static readonly Guid ManagerFinanceView = new Guid("10000000-0000-0000-0003-000000000012");

        /// <summary>
        /// Управление документами
        /// </summary>
        public static readonly Guid ManagerDocuments = new Guid("10000000-0000-0000-0003-000000000013");

        #endregion

        #region Employee (Сотрудник)

        /// <summary>
        /// Просмотр заказов
        /// </summary>
        public static readonly Guid EmployeeOrdersView = new Guid("10000000-0000-0000-0003-000000000020");

        /// <summary>
        /// Работа с назначенными заказами
        /// </summary>
        public static readonly Guid EmployeeOrdersAssigned = new Guid("10000000-0000-0000-0003-000000000021");

        /// <summary>
        /// Просмотр и загрузка чеков
        /// </summary>
        public static readonly Guid EmployeeReceipts = new Guid("10000000-0000-0000-0003-000000000022");

        #endregion
    }
}
