namespace Renoza.Domain.Constants.Company
{
    /// <summary>
    /// Идентификаторы ресурсов компании
    /// </summary>
    public static class CompanyResourceIds
    {
        #region Профиль компании

        /// <summary>
        /// Профиль компании (корневой ресурс)
        /// </summary>
        public static readonly Guid CompanyProfile = new Guid("10000000-0000-0000-0002-000000000001");

        /// <summary>
        /// Информация о компании
        /// </summary>
        public static readonly Guid CompanyProfileInfo = new Guid("10000000-0000-0000-0002-000000000002");

        /// <summary>
        /// Участники компании
        /// </summary>
        public static readonly Guid CompanyProfileMembers = new Guid("10000000-0000-0000-0002-000000000003");

        #endregion

        #region Заказы

        /// <summary>
        /// Заказы (корневой ресурс)
        /// </summary>
        public static readonly Guid Orders = new Guid("10000000-0000-0000-0002-000000000010");

        /// <summary>
        /// Назначенные заказы (заказы, назначенные текущему пользователю)
        /// </summary>
        public static readonly Guid OrdersAssigned = new Guid("10000000-0000-0000-0002-000000000011");

        /// <summary>
        /// Все заказы компании
        /// </summary>
        public static readonly Guid OrdersAll = new Guid("10000000-0000-0000-0002-000000000012");

        #endregion

        #region Финансы

        /// <summary>
        /// Финансы (корневой ресурс)
        /// </summary>
        public static readonly Guid Finance = new Guid("10000000-0000-0000-0002-000000000020");

        /// <summary>
        /// Кассовые чеки
        /// </summary>
        public static readonly Guid FinanceReceipts = new Guid("10000000-0000-0000-0002-000000000021");

        /// <summary>
        /// Финансовые отчеты
        /// </summary>
        public static readonly Guid FinanceReports = new Guid("10000000-0000-0000-0002-000000000022");

        #endregion

        #region Документы

        /// <summary>
        /// Документы (корневой ресурс)
        /// </summary>
        public static readonly Guid Documents = new Guid("10000000-0000-0000-0002-000000000030");

        /// <summary>
        /// Шаблоны документов
        /// </summary>
        public static readonly Guid DocumentsTemplates = new Guid("10000000-0000-0000-0002-000000000031");

        #endregion
    }
}
