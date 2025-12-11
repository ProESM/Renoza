namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Данные, извлеченные из QR кода кассового чека
    /// </summary>
    public class QrCodeData
    {
        /// <summary>
        /// Фискальный номер
        /// </summary>
        public string FiscalNumber { get; set; } = string.Empty;

        /// <summary>
        /// Фискальный документ
        /// </summary>
        public string FiscalDocument { get; set; } = string.Empty;

        /// <summary>
        /// Фискальный признак
        /// </summary>
        public string FiscalSign { get; set; } = string.Empty;

        /// <summary>
        /// Тип операции
        /// </summary>
        public string ReceiptOperationType { get; set; }

        /// <summary>
        /// Дата и время покупки
        /// </summary>
        public DateTime PurchaseDateTime { get; set; }

        /// <summary>
        /// Сумма покупки
        /// </summary>
        public decimal TotalSum { get; set; }
    }
}
