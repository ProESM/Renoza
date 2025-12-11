using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CashReceipts.OfdApi
{
    /// <summary>
    /// Данные чека от OFD.ru
    /// </summary>
    public class OfdReceiptData
    {
        /// <summary>
        /// Версия сериализации документа
        /// </summary>
        [JsonProperty("Version")]
        public int? Version { get; set; }

        /// <summary>
        /// Версия формата фискальных данных
        /// </summary>
        [JsonProperty("DocumentFormat")]
        public string? DocumentFormat { get; set; }

        /// <summary>
        /// Документ (вложенная структура с детальной информацией о чеке)
        /// </summary>
        [JsonProperty("Document")]
        public OfdDocument? Document { get; set; }

        /// <summary>
        /// Численный признак вида документа:
        /// 3 – чек;
        /// 31 – чек коррекции;
        /// 4 – бланк строгой отчетности;
        /// 41 – бланк строгой отчетности коррекции.
        /// </summary>
        [JsonProperty("Tag")]
        public int? Tag { get; set; }

        /// <summary>
        /// ИНН пользователя
        /// </summary>
        [JsonProperty("UserInn")]
        public string? UserInn { get; set; }

        /// <summary>
        /// Регистрационный номер ККТ
        /// </summary>
        [JsonProperty("KktRegNumber")]
        public string? KktRegNumber { get; set; }

        /// <summary>
        /// Заводской номер фискального накопителя
        /// </summary>
        [JsonProperty("FnNumber")]
        public string? FnNumber { get; set; }

        /// <summary>
        /// Фискальный номер документа
        /// </summary>
        [JsonProperty("DocNumber")]
        public int? DocNumber { get; set; }

        /// <summary>
        /// Дата и время формирования документа
        /// </summary>
        [JsonProperty("DocDateTime")]
        public DateTime? DocDateTime { get; set; }

        /// <summary>
        /// Фискальный признак документа
        /// </summary>
        [JsonProperty("DocFiscalSign")]
        public string? DocFiscalSign { get; set; }

        /// <summary>
        /// Фискальный признак документа (десятичный формат)
        /// </summary>
        [JsonProperty("DecimalFiscalSign")]
        public string? DecimalFiscalSign { get; set; }

        /// <summary>
        /// Дата приема документа в систему (UTC)
        /// </summary>
        [JsonProperty("CDateUtc")]
        public DateTime? CDateUtc { get; set; }

        /// <summary>
        /// Схема предупреждения документа принимает значения:
        /// 0 – нет нарушения;
        /// 1 – предупреждение о максимальной длине документа;
        /// 2 – предупреждение о минимальной длине документа;
        /// 3 – предупреждение о фиксированной длине документа.
        /// </summary>
        [JsonProperty("SchemeWarnings")]
        public int? SchemeWarnings { get; set; }

        /// <summary>
        /// Ошибки валидации
        /// </summary>
        [JsonProperty("ValidationErrors")]
        public string? ValidationErrors { get; set; }

        /// <summary>
        /// Адрес установки кассы
        /// </summary>
        [JsonProperty("RegAddresss")]
        public string? RegAddresss { get; set; }

        /// <summary>
        /// Идентификатор ФИАС
        /// </summary>
        [JsonProperty("FiasId")]
        public string? FiasId { get; set; }

        /// <summary>
        /// Географические координаты
        /// </summary>
        [JsonProperty("GeoPoint")]
        public OfdGeoPoint? GeoPoint { get; set; }
    }
}
