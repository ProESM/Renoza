using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Domain.Entities.CompanyVerification.DaDataApi;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Text;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с DaData API
    /// </summary>
    public class DaDataApiService : IDaDataApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DaDataApiService> _logger;
        private readonly CompanyVerificationBrokerOptions _companyVerificationBrokerOptions;

        public DaDataApiService(
            HttpClient httpClient,
            ILogger<DaDataApiService> logger,
            CompanyVerificationBrokerOptions companyVerificationBrokerOptions)
        {
            _httpClient = httpClient;
            _logger = logger;
            _companyVerificationBrokerOptions = companyVerificationBrokerOptions;
        }

        /// <summary>
        /// Получить информацию о компании по ИНН
        /// </summary>
        public async Task<DaDataCompanyResponse?> GetCompanyByInnAsync(string inn, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new DaDataCompanyRequest
                {
                    Query = inn,
                    Count = 1
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Отправка запроса к DaData API: Inn={Inn}", inn);

                // Добавляем заголовки авторизации
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_companyVerificationBrokerOptions.DaDataApiToken}");

                if (!string.IsNullOrEmpty(_companyVerificationBrokerOptions.DaDataApiSecret))
                {
                    _httpClient.DefaultRequestHeaders.Add("X-Secret", _companyVerificationBrokerOptions.DaDataApiSecret);
                }

                var response = await _httpClient.PostAsync(_companyVerificationBrokerOptions.DaDataApiUrl, content, cancellationToken);

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Ошибка вызова DaData API. StatusCode={StatusCode}, Response={Response}",
                        response.StatusCode, responseJson);

                    throw new HttpRequestException($"DaData API returned {response.StatusCode}: {responseJson}");
                }

                var result = JsonConvert.DeserializeObject<DaDataCompanyResponse>(responseJson);

                if (result == null)
                {
                    _logger.LogError("Не удалось десериализовать ответ от DaData API: {Response}", responseJson);
                    throw new InvalidOperationException("Не удалось десериализовать ответ от DaData API");
                }

                _logger.LogInformation("Успешно получены данные о компании от DaData API. Найдено результатов: {Count}",
                    result.Suggestions?.Count ?? 0);

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP ошибка при вызове DaData API для ИНН={Inn}", inn);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Исключение при вызове DaData API для ИНН={Inn}", inn);
                throw;
            }
        }
    }
}
