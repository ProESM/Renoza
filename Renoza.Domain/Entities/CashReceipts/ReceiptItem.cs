namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Позиция чека
    /// </summary>
    public class ReceiptItem
    {
        /// <summary>
        /// Наименование позиции (обязательно)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Единица измерения (опционально, например: "шт", "кг", "л")
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Количество (опционально, по умолчанию 1)
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Цена за единицу (опционально)
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Ставка НДС (опционально, например: 20, 10, 0)
        /// </summary>
        public decimal? VatRate { get; set; }

        /// <summary>
        /// Сумма НДС (опционально)
        /// </summary>
        public decimal? VatAmount { get; set; }

        /// <summary>
        /// Итоговая стоимость с учётом НДС (если применим) (обязательно)
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}
