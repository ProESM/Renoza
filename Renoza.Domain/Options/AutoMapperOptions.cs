namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки лицензии для Lucky Penny AutoMapper Extensions
    /// </summary>
    public class AutoMapperOptions
    {
        /// <summary>
        /// License key для Lucky Penny AutoMapper
        /// Рекомендуется устанавливать через переменную окружения AUTOMAPPER_LICENSE_KEY
        /// или через User Secrets в development режиме
        /// </summary>
        public string? LicenseKey { get; set; }
    }
}
