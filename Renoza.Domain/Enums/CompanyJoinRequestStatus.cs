namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Статусы запроса на вступление в компанию
    /// </summary>
    public enum CompanyJoinRequestStatus : short
    {
        /// <summary>
        /// Ожидает рассмотрения
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Одобрен
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Отклонен
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Отменен пользователем
        /// </summary>
        Cancelled = 4
    }
}
