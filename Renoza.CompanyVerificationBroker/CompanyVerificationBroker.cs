namespace Renoza.CompanyVerificationBroker
{
    /// <summary>
    /// Воркер обработки верификации компаний
    /// </summary>
    public class CompanyVerificationBroker : BackgroundService
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<CompanyVerificationBroker> _logger;

        /// <summary>
        /// Воркер обработки верификации компаний
        /// </summary>
        /// <param name="logger">Логгер</param>
        public CompanyVerificationBroker(ILogger<CompanyVerificationBroker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CompanyVerificationBroker запущен");
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в CompanyVerificationBroker");
            }
        }
    }
}
