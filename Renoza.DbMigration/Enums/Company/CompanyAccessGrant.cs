using Renoza.DbMigration.Attributes.Company;

namespace Renoza.DbMigration.Enums.Company
{
    /// <summary>
    /// Гранты доступа компании
    /// </summary>
    public enum CompanyAccessGrant
    {
        #region Owner (Владелец)

        /// <summary>
        /// Полный доступ владельца компании
        /// </summary>
        [CompanyAccessGrantDetails("10000000-0000-0000-0003-000000000001", "company_owner_full", "Полный доступ владельца компании")]
        OwnerFull,

        #endregion

        #region Manager (Менеджер)

        /// <summary>
        /// Управление заказами
        /// </summary>
        [CompanyAccessGrantDetails("10000000-0000-0000-0003-000000000010", "company_manager_orders", "Управление заказами")]
        ManagerOrders,

        /// <summary>
        /// Управление участниками
        /// </summary>
        [CompanyAccessGrantDetails("10000000-0000-0000-0003-000000000011", "company_manager_members", "Управление участниками")]
        ManagerMembers,

        /// <summary>
        /// Просмотр финансов
        /// </summary>
        [CompanyAccessGrantDetails("10000000-0000-0000-0003-000000000012", "company_manager_finance_view", "Просмотр финансов")]
        ManagerFinanceView,

        /// <summary>
        /// Управление документами
        /// </summary>
        [CompanyAccessGrantDetails("10000000-0000-0000-0003-000000000013", "company_manager_documents", "Управление документами")]
        ManagerDocuments,

        #endregion

        #region Employee (Сотрудник)

        /// <summary>
        /// Просмотр заказов
        /// </summary>
        [CompanyAccessGrantDetails("10000000-0000-0000-0003-000000000020", "company_employee_orders_view", "Просмотр заказов")]
        EmployeeOrdersView,

        /// <summary>
        /// Работа с назначенными заказами
        /// </summary>
        [CompanyAccessGrantDetails("10000000-0000-0000-0003-000000000021", "company_employee_orders_assigned", "Работа с назначенными заказами")]
        EmployeeOrdersAssigned,

        /// <summary>
        /// Просмотр и загрузка чеков
        /// </summary>
        [CompanyAccessGrantDetails("10000000-0000-0000-0003-000000000022", "company_employee_receipts", "Просмотр и загрузка чеков")]
        EmployeeReceipts

        #endregion
    }
}
