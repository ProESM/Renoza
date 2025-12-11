using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CashReceipts.OfdApi
{
    /// <summary>
    /// Товарная позиция в чеке
    /// </summary>
    public class OfdReceiptItem
    {
        /// <summary>
        /// Наименование позиции
        /// </summary>
        [JsonProperty("Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Цена позиции (в копейках)
        /// </summary>
        [JsonProperty("Price")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Количество единиц товарной позиции
        /// </summary>
        [JsonProperty("Quantity")]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// НДС 18% - общая сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds18_TotalSumm")]
        public int? Nds18_TotalSumm { get; set; }

        /// <summary>
        /// НДС 10% - общая сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds10_TotalSumm")]
        public int? Nds10_TotalSumm { get; set; }

        /// <summary>
        /// НДС 0% - общая сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds00_TotalSumm")]
        public int? Nds00_TotalSumm { get; set; }

        /// <summary>
        /// Без НДС - общая сумма (в копейках)
        /// </summary>
        [JsonProperty("NdsNA_TotalSumm")]
        public int? NdsNA_TotalSumm { get; set; }

        /// <summary>
        /// НДС 18% - расчетная сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds18_CalculatedTotalSumm")]
        public int? Nds18_CalculatedTotalSumm { get; set; }

        /// <summary>
        /// НДС 10% - расчетная сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds10_CalculatedTotalSumm")]
        public int? Nds10_CalculatedTotalSumm { get; set; }

        /// <summary>
        /// Общая стоимость позиции (в копейках)
        /// </summary>
        [JsonProperty("Total")]
        public int? Total { get; set; }

        /// <summary>
        /// Скидка/наценка
        /// </summary>
        [JsonProperty("DiscountMarkup")]
        public int? DiscountMarkup { get; set; }

        /// <summary>
        /// Дополнительные реквизиты пользователя
        /// </summary>
        [JsonProperty("Extra")]
        public string? Extra { get; set; }

        /// <summary>
        /// Признак способа расчета.
        /// Параметр может принимать следующие значения:
        /// 1 – предоплата 100%;
        /// 2 – предоплата;
        /// 3 – аванс;
        /// 4 – полный расчет;
        /// 5 – частичный расчет;
        /// 6 – передача в кредит;
        /// 7 – оплата в кредит.
        /// </summary>
        [JsonProperty("CalculationMethod")]
        public int? CalculationMethod { get; set; }

        /// <summary>
        /// Признак предмета расчета
        /// </summary>
        [JsonProperty("SubjectType")]
        public int? SubjectType { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        [JsonProperty("UnitOfMeasure")]
        public string? UnitOfMeasure { get; set; }

        /// <summary>
        /// Код товарной номенклатуры
        /// </summary>
        [JsonProperty("ProductNomenclature")]
        public string? ProductNomenclature { get; set; }

        /// <summary>
        /// Размер НДС за единицу предмета расчета
        /// </summary>
        [JsonProperty("NDS_PieceSumm")]
        public int? NDS_PieceSumm { get; set; }

        /// <summary>
        /// Ставка НДС
        /// 1 - НДС 20%
        /// 2 - НДС 10%;
        /// 3 - 20/120;
        /// 4 - НДС 10/110;
        /// 5 - НДС 0%;
        /// 6 - НДС не облагается.
        /// </summary>
        [JsonProperty("NDS_Rate")]
        public int? NDS_Rate { get; set; }

        /// <summary>
        /// Сумма НДС за предмет расчета
        /// </summary>
        [JsonProperty("NDS_Summ")]
        public int? NDS_Summ { get; set; }

        /// <summary>
        /// Дополнительные реквизиты
        /// </summary>
        [JsonProperty("AdditionalRequisite")]
        public string? AdditionalRequisite { get; set; }
    }
}
