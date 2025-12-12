using Renoza.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Дополнительные метаданные кассового чека
    /// </summary>
    public class CashReceiptMetadata
    {
        /// <summary>
        /// Тип входных данных
        /// </summary>
        [Required(ErrorMessage = "Тип входных данных обязателен")]
        public ReceiptInputType InputType { get; set; }

        /// <summary>
        /// Идентификатор заказчика
        /// </summary>
        [Required(ErrorMessage = "Идентификатор заказчика обязателен")]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Идентификатор заказа (опционально)
        /// </summary>
        public Guid? OrderId { get; set; }
    }
}
