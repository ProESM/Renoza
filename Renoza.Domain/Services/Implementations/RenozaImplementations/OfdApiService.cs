using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Domain.Entities.CashReceipts.OfdApi;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Text;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с API OFD.ru
    /// </summary>
    public class OfdApiService : IOfdApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OfdApiService> _logger;
        private readonly CashReceiptBrokerOptions _cashReceiptBrokerOptions;

        public OfdApiService(
            HttpClient httpClient,
            ILogger<OfdApiService> logger,
            CashReceiptBrokerOptions cashReceiptBrokerOptions)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cashReceiptBrokerOptions = cashReceiptBrokerOptions;
        }

        /// <summary>
        /// Получить данные чека по QR коду
        /// </summary>
        public async Task<OfdApiResponse> GetReceiptAsync(OfdApiRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Устанавливаем токен из конфигурации
                request.TokenSecret = _cashReceiptBrokerOptions.OfdApiToken;

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Отправка запроса к OFD API: FnNumber={FnNumber}, DocNumber={DocNumber}, DocFiscalSign={DocFiscalSign}",
                    request.FnNumber, request.DocNumber, request.DocFiscalSign);

                var response = await _httpClient.PostAsync(_cashReceiptBrokerOptions.OfdApiUrl, content, cancellationToken);

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Ошибка вызова OFD API. StatusCode={StatusCode}, Response={Response}",
                        response.StatusCode, responseJson);

                    return new OfdApiResponse
                    {
                        Success = false,
                        Errors = new List<OfdApiError>
                        {
                            new OfdApiError
                            {
                                Code = "HttpError",
                                Message = $"HTTP {response.StatusCode}: {responseJson}"
                            }
                        }
                    };
                }

                var result = JsonConvert.DeserializeObject<OfdApiResponse>(responseJson);

                if (result == null)
                {
                    _logger.LogError("Не удалось десериализовать ответ от OFD API: {Response}", responseJson);
                    return new OfdApiResponse
                    {
                        Success = false,
                        Errors = new List<OfdApiError>
                        {
                            new OfdApiError
                            {
                                Code = "DeserializationError",
                                Message = "Не удалось десериализовать ответ от OFD API"
                            }
                        }
                    };
                }

                if (result.Success)
                {
                    _logger.LogInformation("Успешно получены данные чека от OFD API. AvailableRequests={AvailableRequests}",
                        result.AvailableRequests);
                }
                else
                {
                    _logger.LogWarning("OFD API вернул ошибку: {Errors}",
                        result.Errors != null ? string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Message}")) : "Unknown");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Исключение при вызове OFD API");
                return new OfdApiResponse
                {
                    Success = false,
                    Errors = new List<OfdApiError>
                    {
                        new OfdApiError
                        {
                            Code = "Exception",
                            Message = ex.Message
                        }
                    }
                };
            }
        }
    }
}
