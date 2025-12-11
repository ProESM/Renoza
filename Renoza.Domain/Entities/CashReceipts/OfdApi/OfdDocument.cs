using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CashReceipts.OfdApi
{
    /// <summary>
    /// Детальная информация о чеке (вложенный объект Document)
    /// </summary>
    public class OfdDocument
    {
        /// <summary>
        /// Наименование документа
        /// </summary>
        [JsonProperty("DocumentName")]
        public string? DocumentName { get; set; }

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
        /// Пользователь
        /// </summary>
        [JsonProperty("User")]
        public string? User { get; set; }

        /// <summary>
        /// ИНН пользователя
        /// </summary>
        [JsonProperty("UserInn")]
        public string? UserInn { get; set; }

        /// <summary>
        /// Номер чека за смену
        /// </summary>
        [JsonProperty("Number")]
        public int? Number { get; set; }

        /// <summary>
        /// Фискальный номер документа
        /// </summary>
        [JsonProperty("Document_Number")]
        public int? Document_Number { get; set; }

        /// <summary>
        /// Дата и время последнего обновления информации о чеке	
        /// </summary>
        [JsonProperty("DateTime")]
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// Номер смены
        /// </summary>
        [JsonProperty("ShiftNumber")]
        public int? ShiftNumber { get; set; }

        /// <summary>
        /// Тип операции (1 – приход, 2 – возврат прихода, 3 – расход, 4 – возврат расхода)
        /// </summary>
        [JsonProperty("OperationType")]
        public int? OperationType { get; set; }

        /// <summary>
        /// Тип налогообложения, принимает значения:
        /// 0 – общая система налогообложения;
        /// 1 – упрощенная система налогообложения(доход);
        /// 2 – упрощенная система налогообложения(доход минус расход);
        /// 3 – единый налог на вмененный доход;
        /// 4 – единый сельскохозяйственный налог;
        /// 5 – патентная система налогообложения.
        /// </summary>
        [JsonProperty("TaxationType")]
        public int? TaxationType { get; set; }

        /// <summary>
        /// Фамилия, имя, отчество оператора
        /// </summary>
        [JsonProperty("Operator")]
        public string? Operator { get; set; }

        /// <summary>
        /// ИНН оператора (кассира)
        /// </summary>
        [JsonProperty("Operator_INN")]
        public string? Operator_INN { get; set; }

        /// <summary>
        /// Регистрационный номер ККТ
        /// </summary>
        [JsonProperty("KKT_RegNumber")]
        public string? KKT_RegNumber { get; set; }

        /// <summary>
        /// Заводской номер ККТ
        /// </summary>
        [JsonProperty("KKT_FactoryNumber")]
        public string? KKT_FactoryNumber { get; set; }

        /// <summary>
        /// Заводской номер автоматического устройства для расчетов
        /// </summary>
        [JsonProperty("KKT_MachineNumber")]
        public string? KKT_MachineNumber { get; set; }

        /// <summary>
        /// Номер фискального накопителя, установленного в кассу (серийный, заводской)
        /// </summary>
        [JsonProperty("FN_FactoryNumber")]
        public string? FN_FactoryNumber { get; set; }

        /// <summary>
        /// Список товаров/услуг в чеке
        /// </summary>
        [JsonProperty("Items")]
        public List<OfdReceiptItem>? Items { get; set; }

        /// <summary>
        /// Список позиций сторно
        /// </summary>
        [JsonProperty("StornoItems")]
        public List<OfdReceiptItem>? StornoItems { get; set; }

        /// <summary>
        /// Общая сумма по чеку (в копейках)
        /// </summary>
        [JsonProperty("Amount_Total")]
        public decimal? Amount_Total { get; set; }

        /// <summary>
        /// Сумма наличными по чеку (в копейках)
        /// </summary>
        [JsonProperty("Amount_Cash")]
        public decimal? Amount_Cash { get; set; }

        /// <summary>
        /// Сумма электронного платежа по чеку (в копейках)
        /// </summary>
        [JsonProperty("Amount_ECash")]
        public decimal? Amount_ECash { get; set; }

        /// <summary>
        /// Сумма предоплатой (зачет аванса)
        /// </summary>
        [JsonProperty("Amount_Advance")]
        public decimal? Amount_Advance { get; set; }

        /// <summary>
        /// Сумма постоплатой (в кредит)
        /// </summary>
        [JsonProperty("Amount_Loan")]
        public decimal? Amount_Loan { get; set; }

        /// <summary>
        /// Сумма встречным предоставлением
        /// </summary>
        [JsonProperty("Amount_Granting")]
        public decimal? Amount_Granting { get; set; }

        /// <summary>
        /// Адрес расположения кассового аппарата
        /// </summary>
        [JsonProperty("RetailPlaceAddress")]
        public string? RetailPlaceAddress { get; set; }

        /// <summary>
        /// Абонентский номер и (или) адрес электронной почты покупателя (клиента)
        /// в случае передачи ему кассового чека (БСО) в электронной форме
        /// </summary>
        [JsonProperty("Buyer_Address")]
        public string? Buyer_Address { get; set; }

        /// <summary>
        /// Адрес электронной почты отправителя чека
        /// </summary>
        [JsonProperty("Sender_Address")]
        public string? Sender_Address { get; set; }

        /// <summary>
        /// Номер телефона платежного агента
        /// </summary>
        [JsonProperty("PaymentAgent_Phone")]
        public string? PaymentAgent_Phone { get; set; }

        /// <summary>
        /// Размер комиссии платежного агента
        /// </summary>
        [JsonProperty("PaymentAgent_Comission")]
        public decimal? PaymentAgent_Comission { get; set; }

        /// <summary>
        /// Номер телефона платежного субагента
        /// </summary>
        [JsonProperty("PaymentSubAgent_Phone")]
        public string? PaymentSubAgent_Phone { get; set; }

        /// <summary>
        /// Номер телефона оператора по приему платежей
        /// </summary>
        [JsonProperty("PaymentOperator_Phone")]
        public string? PaymentOperator_Phone { get; set; }

        /// <summary>
        /// Номер телефона банковского платежного агента
        /// </summary>
        [JsonProperty("BankAgent_Phone")]
        public string? BankAgent_Phone { get; set; }

        /// <summary>
        /// Операция банковского агента
        /// </summary>
        [JsonProperty("BankAgent_Operation")]
        public int? BankAgent_Operation { get; set; }

        /// <summary>
        /// Размер вознаграждения банковского платежного агента
        /// </summary>
        [JsonProperty("BankAgent_Comission")]
        public decimal? BankAgent_Comission { get; set; }

        /// <summary>
        /// Номер телефона банковского платежного субагента
        /// </summary>
        [JsonProperty("BankSubAgent_Phone")]
        public string? BankSubAgent_Phone { get; set; }

        /// <summary>
        /// Операция банковского платежного субагента
        /// </summary>
        [JsonProperty("BankSubAgent_Operation")]
        public string? BankSubAgent_Operation { get; set; }

        /// <summary>
        /// Наименование оператора по переводу денежных средств
        /// </summary>
        [JsonProperty("MoneyOperator_Name")]
        public string? MoneyOperator_Name { get; set; }

        /// <summary>
        /// Номер телефона оператора по переводу денежных средств
        /// </summary>
        [JsonProperty("MoneyOperator_Phone")]
        public string? MoneyOperator_Phone { get; set; }

        /// <summary>
        /// Адрес оператора по переводу денежных средств
        /// </summary>
        [JsonProperty("MoneyOperator_Address")]
        public string? MoneyOperator_Address { get; set; }

        /// <summary>
        /// ИНН оператора по переводу денежных средств
        /// </summary>
        [JsonProperty("MoneyOperator_INN")]
        public string? MoneyOperator_INN { get; set; }

        /// <summary>
        /// НДС 18% - общая сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds18_TotalSumm")]
        public decimal? Nds18_TotalSumm { get; set; }

        /// <summary>
        /// НДС 10% - общая сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds10_TotalSumm")]
        public decimal? Nds10_TotalSumm { get; set; }

        /// <summary>
        /// НДС 0% - общая сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds00_TotalSumm")]
        public decimal? Nds00_TotalSumm { get; set; }

        /// <summary>
        /// Без НДС - общая сумма (в копейках)
        /// </summary>
        [JsonProperty("NdsNA_TotalSumm")]
        public decimal? NdsNA_TotalSumm { get; set; }

        /// <summary>
        /// НДС 18% - расчетная сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds18_CalculatedTotalSumm")]
        public decimal? Nds18_CalculatedTotalSumm { get; set; }

        /// <summary>
        /// НДС 10% - расчетная сумма (в копейках)
        /// </summary>
        [JsonProperty("Nds10_CalculatedTotalSumm")]
        public decimal? Nds10_CalculatedTotalSumm { get; set; }

        /// <summary>
        /// Скидка/наценка
        /// </summary>
        [JsonProperty("DiscountMarkup")]
        public decimal? DiscountMarkup { get; set; }

        /// <summary>
        /// Дополнительные реквизиты
        /// </summary>
        [JsonProperty("AdditionalRequisite")]
        public string? AdditionalRequisite { get; set; }

        /// <summary>
        /// Место осуществления расчетов между пользователем и покупателем (клиентом)
        /// </summary>
        [JsonProperty("Calculation_Place")]
        public string? Calculation_Place { get; set; }

        /// <summary>
        /// Номер телефон поставщика
        /// </summary>
        [JsonProperty("Supplier_Phone")]
        public string? Supplier_Phone { get; set; }

        /// <summary>
        /// Адрес сайта для проверки ФП
        /// </summary>
        [JsonProperty("CheckFP_Site")]
        public string? CheckFP_Site { get; set; }

        /// <summary>
        /// Сайт налогового органа
        /// </summary>
        [JsonProperty("TaxAuthority_Site")]
        public string? TaxAuthority_Site { get; set; }

        /// <summary>
        /// ИНН оператора фискальных данных
        /// </summary>
        [JsonProperty("OfdInn")]
        public string? OfdInn { get; set; }

        /// <summary>
        /// Номер версии формата фискальных документов
        /// </summary>
        [JsonProperty("Format_Version")]
        public int? Format_Version { get; set; }

        /// <summary>
        /// Версия форматов фискальных документов, реализованная в ККТ
        /// </summary>
        [JsonProperty("Format_VersionKKT")]
        public string? Format_VersionKKT { get; set; }

        /// <summary>
        /// Версия форматов фискальных документов, реализованная в ФН
        /// </summary>
        [JsonProperty("Format_VersionFN")]
        public string? Format_VersionFN { get; set; }

        /// <summary>
        /// Применения ККТ в режиме, не предусматривающем обязательной
        /// передачи ФД в налоговые органы в электронной форме через ОФД
        /// </summary>
        [JsonProperty("OfflineMode")]
        public string? OfflineMode { get; set; }

        /// <summary>
        /// Признак применения ККТ в составе автоматического устройства для расчетов
        /// </summary>
        [JsonProperty("AutoMode")]
        public string? AutoMode { get; set; }

        /// <summary>
        /// Осуществления расчетов только в сети «Интернет»,
        /// в которой отсутствует устройство для печати фискальных
        /// документов в составе ККТ
        /// </summary>
        [JsonProperty("InternetSign")]
        public int? InternetSign { get; set; }

        /// <summary>
        /// Признак работы в сфере услуг
        /// </summary>
        [JsonProperty("ServiceSectorSign")]
        public string? ServiceSectorSign { get; set; }

        /// <summary>
        /// Признак ККТ, являющейся автоматизированной системой для БСО
        /// (может формировать только БСО и применяться для осуществления
        /// расчетов только при оказании услуг)
        /// </summary>
        [JsonProperty("StrictFormSign")]
        public string? StrictFormSign { get; set; }

        /// <summary>
        /// Признак передачи фискальных документов в зашифрованном
        /// виде оператору фискальных данных
        /// </summary>
        [JsonProperty("EncryptionSign")]
        public string? EncryptionSign { get; set; }

        /// <summary>
        /// Признак проведения азартных игр
        /// </summary>
        [JsonProperty("GamblingMode")]
        public int? GamblingMode { get; set; }

        /// <summary>
        /// Признак проведения лотереи
        /// </summary>
        [JsonProperty("LotteryMode")]
        public int? LotteryMode { get; set; }

        /// <summary>
        /// Проведение расчётов платежным агентом
        /// </summary>
        [JsonProperty("PaymentAgentMode")]
        public int? PaymentAgentMode { get; set; }

        /// <summary>
        /// Фискальный признак документа
        /// </summary>
        [JsonProperty("FiscalSign")]
        public string? FiscalSign { get; set; }

        /// <summary>
        /// Фискальный признак документа (десятичный формат)
        /// </summary>
        [JsonProperty("DecimalFiscalSign")]
        public string? DecimalFiscalSign { get; set; }

        /// <summary>
        /// Количество кассовых чеков за смену
        /// </summary>
        [JsonProperty("ReceiptsCount")]
        public int? ReceiptsCount { get; set; }

        /// <summary>
        /// Количество фискальных документов за смену
        /// </summary>
        [JsonProperty("DocumentsCount")]
        public int? DocumentsCount { get; set; }

        /// <summary>
        /// Количество ФД, не переданных ОФД
        /// (по которым не было получено подтверждения оператора)
        /// </summary>
        [JsonProperty("BadStateCount")]
        public int? BadStateCount { get; set; }

        /// <summary>
        /// Дата первого ФД из числа не переданных ОФД
        /// </summary>
        [JsonProperty("BadStateDateTime")]
        public DateTime? BadStateDateTime { get; set; }

        /// <summary>
        /// Номер первого ФД из числа не переданных ОФД
        /// </summary>
        [JsonProperty("BadStateNumber")]
        public int? BadStateNumber { get; set; }

        /// <summary>
        /// Подтверждение оператора для переданного фискального
        /// документа отсутствует более двух дней
        /// </summary>
        [JsonProperty("OFD_ResponseTimeout")]
        public string? OFD_ResponseTimeout { get; set; }

        /// <summary>
        /// До истечения срока действия ключей фискального признака
        /// в фискальном накопителе осталось менее 30 дней
        /// </summary>
        [JsonProperty("FiscalDrive_Exhaustion")]
        public string? FiscalDrive_Exhaustion { get; set; }

        /// <summary>
        /// Признак того, что до истечения срока действия ключей
        /// фискального признака в фискальном накопителе осталось менее 3 дней
        /// </summary>
        [JsonProperty("FiscalDrive_ReplaceRequired")]
        public string? FiscalDrive_ReplaceRequired { get; set; }

        /// <summary>
        /// Признак того, что память фискального накопителя заполнена более чем на 99%
        /// </summary>
        [JsonProperty("FiscalDrive_MemoryExceeded")]
        public bool? FiscalDrive_MemoryExceeded { get; set; }

        /// <summary>
        /// Причина изменения сведений о ККТ
        /// </summary>
        [JsonProperty("ReRegReasons")]
        public string? ReRegReasons { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        [JsonProperty("Messages")]
        public string? Messages { get; set; }

        /// <summary>
        /// Дополнительные реквизиты пользователя
        /// </summary>
        [JsonProperty("Extra")]
        public string? Extra { get; set; }

        /// <summary>
        /// Тип коррекции:
        /// Самостоятельно – 0;
        /// По предписанию – 1.
        /// </summary>
        [JsonProperty("Correction_Type")]
        public int? Correction_Type { get; set; }

        /// <summary>
        /// Основание для коррекции
        /// </summary>
        [JsonProperty("Correction")]
        public string? Correction { get; set; }

        /// <summary>
        /// Признак ККТ, предназначенной для применения только
        /// в составе автоматического устройства для расчетов
        /// </summary>
        [JsonProperty("Sign_KKT_Machine")]
        public int? Sign_KKT_Machine { get; set; }

        /// <summary>
        /// Продажа подакцизного товара
        /// </summary>
        [JsonProperty("Sign_Excise")]
        public int? Sign_Excise { get; set; }

        /// <summary>
        /// Адрес сайта, на котором покупатель может бесплатно получить чек
        /// </summary>
        [JsonProperty("RecipeSite")]
        public string? RecipeSite { get; set; }

        /// <summary>
        /// Срок действия ключей фискального признака
        /// (величина учитывается в днях до момента истечения срока действия ключей)
        /// </summary>
        [JsonProperty("ValidityPeriod")]
        public string? ValidityPeriod { get; set; }

        /// <summary>
        /// Итоговая суммы расчетов, указанная в чеке
        /// </summary>
        [JsonProperty("ShiftTotals")]
        public decimal? ShiftTotals { get; set; }

        /// <summary>
        /// Итоговые количества и итоговые суммы расчетов фискальных данных
        /// </summary>
        [JsonProperty("DeliveredTotals")]
        public string? DeliveredTotals { get; set; }

        /// <summary>
        /// Итоговые количества и итоговые суммы расчетов непереданных фискальных данных
        /// </summary>
        [JsonProperty("UndeliveredTotals")]
        public string? UndeliveredTotals { get; set; }
    }
}
