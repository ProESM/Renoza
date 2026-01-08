using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Валюта
    /// </summary>
    public enum Currency
    {
        #region СНГ + Украина и Грузия

        /// <summary>
        /// Российский рубль
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000001", "RUB", "Российский рубль", "₽", true)]
        RussianRuble,
        /// <summary>
        /// Белорусский рубль
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000002", "BYN", "Белорусский рубль", "Br", true)]
        BelarusianRuble,
        /// <summary>
        /// Тенге
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000003", "KZT", "Тенге", "₸", true)]
        Tenge,
        /// <summary>
        /// Армянский драм
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000004", "AMD", "Армянский драм", "֏", true)]
        ArmenianDram,
        /// <summary>
        /// Азербайджанский манат
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000005", "AZN", "Азербайджанский манат", "₼", true)]
        AzerbaijaniManat,
        /// <summary>
        /// Узбекский сум
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000006", "UZS", "Узбекский сум", "soʻm", true)]
        UzbekistaniSom,
        /// <summary>
        /// Киргизский сом
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000007", "KGS", "Киргизский сом", "с", true)]
        KyrgyzstaniSom,
        /// <summary>
        /// Таджикский сомони
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000008", "TJS", "Таджикский сомони", "ЅМ", true)]
        TajikistaniSomoni,
        /// <summary>
        /// Туркменский манат
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000009", "TMT", "Туркменский манат", "m", true)]
        TurkmenistanManat,
        /// <summary>
        /// Молдавский лей
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000010", "MDL", "Молдавский лей", "L", true)]
        MoldovanLeu,
        /// <summary>
        /// Гривна
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000011", "UAH", "Гривна", "₴", true)]
        UkrainianHryvnia,
        /// <summary>
        /// Лари
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000012", "GEL", "Лари", "₾", true)]
        GeorgianLari,

        #endregion

        #region Доллар США и евро

        /// <summary>
        /// Доллар США
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000013", "USD", "Доллар США", "$", true)]
        UnitedStatesDollar,
        /// <summary>
        /// Евро
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000014", "EUR", "Евро", "€", true)]
        Euro,

        #endregion
        
        #region Основные мировые валюты

        /// <summary>
        /// Фунт стерлингов
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000015", "GBP", "Фунт стерлингов", "£", true)]
        PoundSterling,
        /// <summary>
        /// Японская иена
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000016", "JPY", "Японская иена", "¥", true)]
        JapaneseYen,
        /// <summary>
        /// Китайский юань
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000017", "CNY", "Китайский юань", "¥", true)]
        ChineseYuan,
        /// <summary>
        /// Швейцарский франк
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000018", "CHF", "Швейцарский франк", "CHF", true)]
        SwissFranc,
        /// <summary>
        /// Канадский доллар
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000019", "CAD", "Канадский доллар", "$", true)]
        CanadianDollar,
        /// <summary>
        /// Австралийский доллар
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000020", "AUD", "Австралийский доллар", "$", true)]
        AustralianDollar,
        /// <summary>
        /// Новозеландский доллар
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000021", "NZD", "Новозеландский доллар", "$", true)]
        NewZealandDollar,
        /// <summary>
        /// Южнокорейская вона
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000022", "KRW", "Южнокорейская вона", "₩", true)]
        SouthKoreanWon,
        /// <summary>
        /// Индийская рупия
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000023", "INR", "Индийская рупия", "₹", true)]
        IndianRupee,
        /// <summary>
        /// Бразильский реал
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000024", "BRL", "Бразильский реал", "R$", true)]
        BrazilianReal,
        /// <summary>
        /// Турецкая лира
        /// </summary>
        [CurrencyDetails("00000000-0000-0000-0000-000000000025", "TRY", "Турецкая лира", "₺", true)]
        TurkishLira

        #endregion
    }
}
