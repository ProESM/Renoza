using Renoza.DbMigration.Attributes.Company;

namespace Renoza.DbMigration.Enums.Company
{
    /// <summary>
    /// Статусы запроса на вступление в компанию
    /// </summary>
    public enum CompanyJoinRequestStatus : short
    {
        /// <summary>
        /// Ожидает рассмотрения
        /// </summary>
        [CompanyJoinRequestStatusDetails(1, "pending", "Ожидает рассмотрения", "Запрос ожидает рассмотрения", true)]
        Pending = 1,

        /// <summary>
        /// Одобрен
        /// </summary>
        [CompanyJoinRequestStatusDetails(2, "approved", "Одобрен", "Запрос одобрен", true)]
        Approved = 2,

        /// <summary>
        /// Отклонен
        /// </summary>
        [CompanyJoinRequestStatusDetails(3, "rejected", "Отклонен", "Запрос отклонен", true)]
        Rejected = 3,

        /// <summary>
        /// Отменен пользователем
        /// </summary>
        [CompanyJoinRequestStatusDetails(4, "cancelled", "Отменен", "Запрос отменен пользователем", true)]
        Cancelled = 4
    }
}
