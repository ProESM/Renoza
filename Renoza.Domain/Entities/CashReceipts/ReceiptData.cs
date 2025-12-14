namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Данные чека
    /// </summary>
    public class ReceiptData
    {
        /// <summary>
        /// Позиции чека (опционально, может быть указана только общая сумма)
        /// </summary>
        public List<ReceiptItem>? Items { get; set; }

        /// <summary>
        /// Общая сумма чека (вычисляется автоматически из позиций или задаётся вручную)
        /// </summary>
        public decimal? TotalSum { get; set; }

        /// <summary>
        /// Дата и время покупки (опционально)
        /// </summary>
        public DateTime? PurchaseDateTime { get; set; }

        /// <summary>
        /// Название организации (опционально)
        /// </summary>
        public string? OrganizationName { get; set; }

        /// <summary>
        /// ИНН организации (опционально)
        /// </summary>
        public string? OrganizationInn { get; set; }

        /// <summary>
        /// Адрес точки продажи (опционально)
        /// </summary>
        public string? RetailPlaceAddress { get; set; }
    }
}
