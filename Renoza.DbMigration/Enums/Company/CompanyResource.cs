using Renoza.DbMigration.Attributes.Company;

namespace Renoza.DbMigration.Enums.Company
{
    /// <summary>
    /// Ресурсы компании
    /// </summary>
    public enum CompanyResource
    {
        #region Профиль компании

        /// <summary>
        /// Профиль компании (корневой ресурс)
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000001", "company_profile", "Профиль компании")]
        CompanyProfile,

        /// <summary>
        /// Информация о компании
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000002", "company_profile.info", "Информация о компании", "10000000-0000-0000-0002-000000000001")]
        CompanyProfileInfo,

        /// <summary>
        /// Участники компании
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000003", "company_profile.members", "Участники компании", "10000000-0000-0000-0002-000000000001")]
        CompanyProfileMembers,

        #endregion

        #region Заказы

        /// <summary>
        /// Заказы (корневой ресурс)
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000010", "orders", "Заказы")]
        Orders,

        /// <summary>
        /// Назначенные заказы (заказы, назначенные текущему пользователю)
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000011", "orders.assigned", "Назначенные заказы", "10000000-0000-0000-0002-000000000010")]
        OrdersAssigned,

        /// <summary>
        /// Все заказы компании
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000012", "orders.all", "Все заказы компании", "10000000-0000-0000-0002-000000000010")]
        OrdersAll,

        #endregion

        #region Финансы

        /// <summary>
        /// Финансы (корневой ресурс)
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000020", "finance", "Финансы")]
        Finance,

        /// <summary>
        /// Кассовые чеки
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000021", "finance.receipts", "Кассовые чеки", "10000000-0000-0000-0002-000000000020")]
        FinanceReceipts,

        /// <summary>
        /// Финансовые отчеты
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000022", "finance.reports", "Финансовые отчеты", "10000000-0000-0000-0002-000000000020")]
        FinanceReports,

        #endregion

        #region Документы

        /// <summary>
        /// Документы (корневой ресурс)
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000030", "documents", "Документы")]
        Documents,

        /// <summary>
        /// Шаблоны документов
        /// </summary>
        [CompanyResourceDetails("10000000-0000-0000-0002-000000000031", "documents.templates", "Шаблоны документов", "10000000-0000-0000-0002-000000000030")]
        DocumentsTemplates

        #endregion
    }
}
