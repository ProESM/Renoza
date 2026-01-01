namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки брокера верификации компаний
    /// </summary>
    public class CompanyVerificationBrokerOptions
    {
        /// <summary>
        /// Имя очереди для валидации заданий на верификацию компаний
        /// </summary>
        public string CompanyVerificationValidationConsumerQueueName { get; set; } = string.Empty;

        /// <summary>
        /// Имя очереди для обработки верификации компаний
        /// </summary>
        public string CompanyVerificationProcessingConsumerQueueName { get; set; } = string.Empty;

        /// <summary>
        /// Имя очереди для сохранения результатов верификации
        /// </summary>
        public string CompanyVerificationSaveConsumerQueueName { get; set; } = string.Empty;

        /// <summary>
        /// URL API DaData для получения информации о компании
        /// </summary>
        public string DaDataApiUrl { get; set; } = "https://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/party";

        /// <summary>
        /// Токен для доступа к DaData API
        /// </summary>
        public string DaDataApiToken { get; set; } = string.Empty;

        /// <summary>
        /// Secret ключ для доступа к DaData API (опционально, для платных тарифов)
        /// </summary>
        public string? DaDataApiSecret { get; set; }
    }
}
