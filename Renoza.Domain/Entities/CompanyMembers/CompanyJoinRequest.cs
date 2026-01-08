namespace Renoza.Domain.Entities.CompanyMembers
{
    /// <summary>
    /// Запрос на вступление в компанию
    /// </summary>
    public class CompanyJoinRequest
    {
        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, который запрашивает вступление
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор профиля компании
        /// </summary>
        public Guid CompanyProfileId { get; set; }

        /// <summary>
        /// Статус запроса (Pending, Approved, Rejected)
        /// </summary>
        public short StatusId { get; set; }

        /// <summary>
        /// Комментарий к запросу от пользователя
        /// </summary>
        public string? RequestMessage { get; set; }

        /// <summary>
        /// Комментарий при одобрении/отклонении от владельца/менеджера
        /// </summary>
        public string? ResponseMessage { get; set; }

        /// <summary>
        /// Дата создания запроса
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата обновления запроса
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// ID пользователя, который рассмотрел запрос (владелец/менеджер компании)
        /// </summary>
        public Guid? ReviewerId { get; set; }

        /// <summary>
        /// Дата рассмотрения запроса
        /// </summary>
        public DateTime? ReviewedAt { get; set; }
    }
}
