namespace Renoza.DbMigration.Attributes
{
    /// <summary>
    /// Атрибут для описания валют
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CurrencyDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Уникальный код валюты (RUB, USD, EUR и т.д.)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Наименование (Российский рубль, Доллар США, Евро)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Символ валюты (₽, $, €)
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Атрибут для описания валют
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="code">Уникальный код</param>
        /// <param name="name">Наименование</param>
        /// <param name="symbol">Символ валюты</param>
        /// <param name="isActive">Признак активности</param>
        public CurrencyDetailsAttribute(string id, string code, string name, string symbol, bool isActive = true)
        {
            Id = new Guid(id);
            Code = code;
            Name = name;
            Symbol = symbol;
            IsActive = isActive;
        }
    }
}
