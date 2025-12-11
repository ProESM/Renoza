using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Domain.Entities.CashReceipts.OfdApi;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.CashReceipt;
using Renoza.Domain.Options;
using Renoza.Domain.QueueConsumers.Interfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Domain.QueueConsumers.Implementations.CashReceiptBroker
{
    /// <summary>
    /// Consumer для распознавания кассового чека через внешний API
    /// </summary>
    public class CashReceiptRecognitionConsumer : IQueueConsumer<CashReceiptRecognitionMessage>
    {
        private readonly ILogger<CashReceiptRecognitionConsumer> _logger;
        private readonly IBus _bus;
        private readonly CashReceiptBrokerOptions _cashReceiptBrokerOptions;
        private readonly ICashReceiptJobService _cashReceiptJobService;
        private readonly IOfdApiService _ofdApiService;

        public CashReceiptRecognitionConsumer(
            ILogger<CashReceiptRecognitionConsumer> logger,
            IBus bus,
            CashReceiptBrokerOptions cashReceiptBrokerOptions,
            ICashReceiptJobService cashReceiptJobService,
            IOfdApiService ofdApiService)
        {
            _logger = logger;
            _bus = bus;
            _cashReceiptBrokerOptions = cashReceiptBrokerOptions;
            _cashReceiptJobService = cashReceiptJobService;
            _ofdApiService = ofdApiService;
        }

        public async Task Consume(ConsumeContext<CashReceiptRecognitionMessage> context)
        {
            await ProcessMessage(context.Message);
        }

        private async Task ProcessMessage(CashReceiptRecognitionMessage message)
        {
            _logger.LogInformation($"Начато распознавание чека через OFD API. JobId: {message.JobId}");

            try
            {
                // Обновляем статус на Processing
                await _cashReceiptJobService.UpdateJobStatusAsync(
                    message.JobId,
                    CashReceiptJobStatus.Processing,
                    "Распознавание чека");

                var totalSum = (int)Math.Round(message.TotalSum * 100);
                // Формируем запрос к OFD API
                var ofdRequest = new OfdApiRequest
                {
                    TotalSum = totalSum,
                    DocDateTime = message.PurchaseDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    FnNumber = message.FiscalNumber,
                    ReceiptOperationType = message.ReceiptOperationType,
                    DocNumber = message.FiscalDocument,
                    DocFiscalSign = message.FiscalSign
                };

                // Вызываем OFD API
                var ofdResponse = await _ofdApiService.GetReceiptAsync(ofdRequest); //TODO нужно раскомментировать, когда подключим внешний сервис

                // TODO Временно замокаем данные, но нужно будет убрать, когда подключим внешний сервис
                //var ofdResponse = GetMockOfdApiResponse();

                if (!ofdResponse.Success || ofdResponse.Data == null)
                {
                    var errorMessage = ofdResponse.Errors != null && ofdResponse.Errors.Any()
                        ? string.Join("; ", ofdResponse.Errors.Select(e => $"{e.Code}: {e.Message}"))
                        : "Неизвестная ошибка при получении данных от OFD API";

                    _logger.LogError($"JobId: {message.JobId}: Ошибка от OFD API: {errorMessage}");

                    await _cashReceiptJobService.UpdateJobStatusAsync(
                        message.JobId,
                        CashReceiptJobStatus.RecognitionFailed,
                        $"Ошибка от OFD API: {errorMessage}");

                    return;
                }

                // Сериализуем полученные данные в JSON
                var receiptJson = JsonConvert.SerializeObject(ofdResponse.Data);

                _logger.LogInformation($"JobId: {message.JobId}: Чек успешно распознан через OFD API");

                // Обновляем статус на Recognized
                await _cashReceiptJobService.UpdateJobStatusAsync(
                    message.JobId,
                    CashReceiptJobStatus.Recognized,
                    $"Чек успешно распознан");

                // Обновляем статус на Saving
                await _cashReceiptJobService.UpdateJobStatusAsync(
                    message.JobId,
                    CashReceiptJobStatus.Saving,
                    "Сохранение распознанного чека в БД");

                // Отправляем сообщение в очередь сохранения
                var saveMessage = new CashReceiptSaveMessage
                {
                    JobId = message.JobId,
                    ReceiptJson = receiptJson
                };

                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_cashReceiptBrokerOptions.CashReceiptSaveConsumerQueueName}"));
                await endpoint.Send(saveMessage);

                _logger.LogInformation($"JobId: {message.JobId}: Отправлено на сохранение");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при распознавании чека. JobId: {message.JobId}");

                await _cashReceiptJobService.UpdateJobStatusAsync(
                    message.JobId,
                    CashReceiptJobStatus.RecognitionFailed,
                    $"Ошибка распознавания: {ex.Message}");

                throw;
            }
        }

        private OfdApiResponse GetMockOfdApiResponse()
        {
            //var ofdApiResponseJson = @"{""Success"": true, ""Data"": {""Version"":2,""DocumentFormat"":""Undefined"",""Document"":{""DocumentName"":null,""Tag"":3,""User"":""АПЕРЯН МГЕР САМСОНОВИЧ"",""UserInn"":""775110717072"",""Number"":34,""DateTime"":""2025-10-03T15:29:00"",""ShiftNumber"":478,""OperationType"":1,""TaxationType"":null,""Operator"":""продавец-кассир Бучнев Андрей"",""KKT_RegNumber"":""0005492800009326    "",""FN_FactoryNumber"":""7381440700213533"",""Items"":[{""Name"":""Утеплитель Пеноплекс 1200х600х50мм./1 лист"",""Price"":43500,""Quantity"":2.0,""Nds18_TotalSumm"":null,""Nds10_TotalSumm"":null,""Nds00_TotalSumm"":null,""NdsNA_TotalSumm"":null,""Nds18_CalculatedTotalSumm"":null,""Nds10_CalculatedTotalSumm"":null,""Total"":87000,""DiscountMarkup"":null,""Extra"":null,""CalculationMethod"":4,""SubjectType"":null,""UnitOfMeasure"":null,""ProductNomenclature"":null,""NDS_PieceSumm"":null,""NDS_Rate"":6,""NDS_Summ"":87000,""AdditionalRequisite"":null},{""Name"":""Крестики д/плитки 1,5мм/200шт"",""Price"":5000,""Quantity"":1.0,""Nds18_TotalSumm"":null,""Nds10_TotalSumm"":null,""Nds00_TotalSumm"":null,""NdsNA_TotalSumm"":null,""Nds18_CalculatedTotalSumm"":null,""Nds10_CalculatedTotalSumm"":null,""Total"":5000,""DiscountMarkup"":null,""Extra"":null,""CalculationMethod"":4,""SubjectType"":null,""UnitOfMeasure"":null,""ProductNomenclature"":null,""NDS_PieceSumm"":null,""NDS_Rate"":6,""NDS_Summ"":5000,""AdditionalRequisite"":null},{""Name"":""Валик \""Велюр\"" Matrix сменный 100мм.D-16мм, руч.-6мм 80616"",""Price"":8000,""Quantity"":4.0,""Nds18_TotalSumm"":null,""Nds10_TotalSumm"":null,""Nds00_TotalSumm"":null,""NdsNA_TotalSumm"":null,""Nds18_CalculatedTotalSumm"":null,""Nds10_CalculatedTotalSumm"":null,""Total"":32000,""DiscountMarkup"":null,""Extra"":null,""CalculationMethod"":4,""SubjectType"":null,""UnitOfMeasure"":null,""ProductNomenclature"":null,""NDS_PieceSumm"":null,""NDS_Rate"":6,""NDS_Summ"":32000,""AdditionalRequisite"":null},{""Name"":""Ручка для валика 100 мм. (6 мм.) 80578"",""Price"":6500,""Quantity"":1.0,""Nds18_TotalSumm"":null,""Nds10_TotalSumm"":null,""Nds00_TotalSumm"":null,""NdsNA_TotalSumm"":null,""Nds18_CalculatedTotalSumm"":null,""Nds10_CalculatedTotalSumm"":null,""Total"":6500,""DiscountMarkup"":null,""Extra"":null,""CalculationMethod"":4,""SubjectType"":null,""UnitOfMeasure"":null,""ProductNomenclature"":null,""NDS_PieceSumm"":null,""NDS_Rate"":6,""NDS_Summ"":6500,""AdditionalRequisite"":null}],""StornoItems"":null,""RetailPlaceAddress"":""108814, г.Москва, п.Сосенское, ул. Сервантеса, д. 3, к. 3"",""Buyer_Address"":null,""Sender_Address"":null,""PaymentAgent_Phone"":null,""MoneyOperator_Phone"":null,""BankAgent_Phone"":null,""BankAgent_Operation"":null,""BankAgent_Comission"":null,""MoneyOperator_Name"":null,""MoneyOperator_Address"":null,""MoneyOperator_INN"":null,""Nds18_TotalSumm"":0,""Nds10_TotalSumm"":0,""Nds00_TotalSumm"":0,""NdsNA_TotalSumm"":130500,""Nds18_CalculatedTotalSumm"":0,""Nds10_CalculatedTotalSumm"":0,""Amount_Total"":130500,""Amount_Cash"":130500,""Amount_ECash"":0,""Document_Number"":41304,""FiscalSign"":""AADvljO1"",""DecimalFiscalSign"":""4019598261"",""ReceiptsCount"":null,""DocumentsCount"":null,""BadStateCount"":null,""BadStateDateTime"":null,""OFD_ResponseTimeout"":null,""FiscalDrive_Exhaustion"":null,""OfflineMode"":null,""BadStateNumber"":null,""StrictFormSign"":null,""ServiceSectorSign"":null,""EncryptionSign"":null,""AutoMode"":null,""KKT_MachineNumber"":null,""InternetSign"":null,""OfdInn"":null,""KKT_FactoryNumber"":null,""FiscalDrive_ReplaceRequired"":null,""FiscalDrive_MemoryExceeded"":null,""ReRegReasons"":null,""CheckFP_Site"":null,""PaymentSubAgent_Phone"":null,""PaymentOperator_Phone"":null,""PaymentAgent_Comission"":null,""BankSubAgent_Phone"":null,""BankSubAgent_Operation"":null,""Messages"":null,""DiscountMarkup"":null,""Extra"":null,""Format_Version"":null,""Format_VersionKKT"":null,""Format_VersionFN"":null,""Correction_Type"":null,""Correction"":null,""Amount_Advance"":0,""Amount_Loan"":0,""Amount_Granting"":0,""Supplier_Phone"":null,""TaxAuthority_Site"":null,""AdditionalRequisite"":null,""ShiftTotals"":null,""DeliveredTotals"":null,""UndeliveredTotals"":null,""Calculation_Place"":null,""GamblingMode"":null,""PaymentAgentMode"":null,""LotteryMode"":null,""Sign_KKT_Machine"":null,""Sign_Excise"":null,""Operator_INN"":null,""RecipeSite"":null,""ValidityPeriod"":null},""Tag"":0,""UserInn"":""775110717072"",""KktRegNumber"":""0005492800009326    "",""FnNumber"":""7381440700213533"",""DocNumber"":41304,""DocDateTime"":""2025-10-03T15:29:00"",""DocFiscalSign"":""tTOW7wAA"",""DecimalFiscalSign"":""4019598261"",""CDateUtc"":""2025-10-03T15:29:00"",""SchemeWarnings"":null,""ValidationErrors"":null,""RegAddresss"":""г. Москва, ул. Сервантеса, д. 3, к. 3"",""FiasId"":""6a5380d2-728d-4cc5-8ddd-fa82ecdca06b"",""GeoPoint"":{""Latitude"":55.591264,""Longitude"":37.453603}}, ""AvailableRequests"": 143 }";
            var ofdApiResponseJson = @"{
    ""Success"": true,
    ""Data"": {
        ""Version"": 2,
        ""DocumentFormat"": ""Undefined"",
        ""Document"": {
            ""DocumentName"": null,
            ""Tag"": 3,
            ""User"": ""ООО \""Лента\"""",
            ""UserInn"": ""7814148471"",
            ""Number"": 4,
            ""DateTime"": ""2025-10-23T09:30:00"",
            ""ShiftNumber"": 266,
            ""OperationType"": 1,
            ""TaxationType"": null,
            ""Operator"": ""- Кубанова"",
            ""KKT_RegNumber"": ""0005850057026850    "",
            ""FN_FactoryNumber"": ""7380440801197696"",
            ""Items"": [
                {
                    ""Name"": ""Молоко ПРОСТОКВАШ отб ПЭТ 3,4-4,5% 930мл"",
                    ""Price"": 9999,
                    ""Quantity"": 1.0,
                    ""Nds18_TotalSumm"": null,
                    ""Nds10_TotalSumm"": null,
                    ""Nds00_TotalSumm"": null,
                    ""NdsNA_TotalSumm"": null,
                    ""Nds18_CalculatedTotalSumm"": null,
                    ""Nds10_CalculatedTotalSumm"": null,
                    ""Total"": 9999,
                    ""DiscountMarkup"": null,
                    ""Extra"": null,
                    ""CalculationMethod"": 4,
                    ""SubjectType"": null,
                    ""UnitOfMeasure"": null,
                    ""ProductNomenclature"": null,
                    ""NDS_PieceSumm"": null,
                    ""NDS_Rate"": 2,
                    ""NDS_Summ"": 909,
                    ""AdditionalRequisite"": null
                },
                {
                    ""Name"": ""Хлопья ЯСНО СОЛН рисовые 375г"",
                    ""Price"": 13499,
                    ""Quantity"": 1.0,
                    ""Nds18_TotalSumm"": null,
                    ""Nds10_TotalSumm"": null,
                    ""Nds00_TotalSumm"": null,
                    ""NdsNA_TotalSumm"": null,
                    ""Nds18_CalculatedTotalSumm"": null,
                    ""Nds10_CalculatedTotalSumm"": null,
                    ""Total"": 13499,
                    ""DiscountMarkup"": null,
                    ""Extra"": null,
                    ""CalculationMethod"": 4,
                    ""SubjectType"": null,
                    ""UnitOfMeasure"": null,
                    ""ProductNomenclature"": null,
                    ""NDS_PieceSumm"": null,
                    ""NDS_Rate"": 2,
                    ""NDS_Summ"": 1227,
                    ""AdditionalRequisite"": null
                },
                {
                    ""Name"": ""Журнал ЧУДЕС.ЖУРНАЛ Леди Баг и Супер-кот"",
                    ""Price"": 39999,
                    ""Quantity"": 1.0,
                    ""Nds18_TotalSumm"": null,
                    ""Nds10_TotalSumm"": null,
                    ""Nds00_TotalSumm"": null,
                    ""NdsNA_TotalSumm"": null,
                    ""Nds18_CalculatedTotalSumm"": null,
                    ""Nds10_CalculatedTotalSumm"": null,
                    ""Total"": 39999,
                    ""DiscountMarkup"": null,
                    ""Extra"": null,
                    ""CalculationMethod"": 4,
                    ""SubjectType"": null,
                    ""UnitOfMeasure"": null,
                    ""ProductNomenclature"": null,
                    ""NDS_PieceSumm"": null,
                    ""NDS_Rate"": 2,
                    ""NDS_Summ"": 3636,
                    ""AdditionalRequisite"": null
                },
                {
                    ""Name"": ""Масло слив СЕВ МОЛОКО Трад Вол 82,5%180г"",
                    ""Price"": 24999,
                    ""Quantity"": 1.0,
                    ""Nds18_TotalSumm"": null,
                    ""Nds10_TotalSumm"": null,
                    ""Nds00_TotalSumm"": null,
                    ""NdsNA_TotalSumm"": null,
                    ""Nds18_CalculatedTotalSumm"": null,
                    ""Nds10_CalculatedTotalSumm"": null,
                    ""Total"": 24999,
                    ""DiscountMarkup"": null,
                    ""Extra"": null,
                    ""CalculationMethod"": 4,
                    ""SubjectType"": null,
                    ""UnitOfMeasure"": null,
                    ""ProductNomenclature"": null,
                    ""NDS_PieceSumm"": null,
                    ""NDS_Rate"": 2,
                    ""NDS_Summ"": 2273,
                    ""AdditionalRequisite"": null
                },
                {
                    ""Name"": ""Фарш САМСОН Домашний кат.Б охл 400г"",
                    ""Price"": 27999,
                    ""Quantity"": 1.0,
                    ""Nds18_TotalSumm"": null,
                    ""Nds10_TotalSumm"": null,
                    ""Nds00_TotalSumm"": null,
                    ""NdsNA_TotalSumm"": null,
                    ""Nds18_CalculatedTotalSumm"": null,
                    ""Nds10_CalculatedTotalSumm"": null,
                    ""Total"": 27999,
                    ""DiscountMarkup"": null,
                    ""Extra"": null,
                    ""CalculationMethod"": 4,
                    ""SubjectType"": null,
                    ""UnitOfMeasure"": null,
                    ""ProductNomenclature"": null,
                    ""NDS_PieceSumm"": null,
                    ""NDS_Rate"": 2,
                    ""NDS_Summ"": 2545,
                    ""AdditionalRequisite"": null
                },
                {
                    ""Name"": ""Конфета жевательная MAMBA 79,5г"",
                    ""Price"": 9999,
                    ""Quantity"": 1.0,
                    ""Nds18_TotalSumm"": null,
                    ""Nds10_TotalSumm"": null,
                    ""Nds00_TotalSumm"": null,
                    ""NdsNA_TotalSumm"": null,
                    ""Nds18_CalculatedTotalSumm"": null,
                    ""Nds10_CalculatedTotalSumm"": null,
                    ""Total"": 9999,
                    ""DiscountMarkup"": null,
                    ""Extra"": null,
                    ""CalculationMethod"": 4,
                    ""SubjectType"": null,
                    ""UnitOfMeasure"": null,
                    ""ProductNomenclature"": null,
                    ""NDS_PieceSumm"": null,
                    ""NDS_Rate"": 1,
                    ""NDS_Summ"": 1667,
                    ""AdditionalRequisite"": null
                },
                {
                    ""Name"": ""Карта-сюрприз"",
                    ""Price"": 0,
                    ""Quantity"": 1.0,
                    ""Nds18_TotalSumm"": null,
                    ""Nds10_TotalSumm"": null,
                    ""Nds00_TotalSumm"": null,
                    ""NdsNA_TotalSumm"": null,
                    ""Nds18_CalculatedTotalSumm"": null,
                    ""Nds10_CalculatedTotalSumm"": null,
                    ""Total"": 0,
                    ""DiscountMarkup"": null,
                    ""Extra"": null,
                    ""CalculationMethod"": 4,
                    ""SubjectType"": null,
                    ""UnitOfMeasure"": null,
                    ""ProductNomenclature"": null,
                    ""NDS_PieceSumm"": null,
                    ""NDS_Rate"": 1,
                    ""NDS_Summ"": 0,
                    ""AdditionalRequisite"": null
                }
            ],
            ""StornoItems"": null,
            ""RetailPlaceAddress"": ""Россия,108814,Москва г.,поселение Сосенское вн.тер.г. Веласкеса б-р,д.3,к.1"",
            ""Buyer_Address"": null,
            ""Sender_Address"": null,
            ""PaymentAgent_Phone"": null,
            ""MoneyOperator_Phone"": null,
            ""BankAgent_Phone"": null,
            ""BankAgent_Operation"": null,
            ""BankAgent_Comission"": null,
            ""MoneyOperator_Name"": null,
            ""MoneyOperator_Address"": null,
            ""MoneyOperator_INN"": null,
            ""Nds18_TotalSumm"": 1667,
            ""Nds10_TotalSumm"": 10590,
            ""Nds00_TotalSumm"": 0,
            ""NdsNA_TotalSumm"": 0,
            ""Nds18_CalculatedTotalSumm"": 0,
            ""Nds10_CalculatedTotalSumm"": 0,
            ""Amount_Total"": 126494,
            ""Amount_Cash"": 0,
            ""Amount_ECash"": 126494,
            ""Document_Number"": 27799,
            ""FiscalSign"": ""AAAf0MoB"",
            ""DecimalFiscalSign"": ""533776897"",
            ""ReceiptsCount"": null,
            ""DocumentsCount"": null,
            ""BadStateCount"": null,
            ""BadStateDateTime"": null,
            ""OFD_ResponseTimeout"": null,
            ""FiscalDrive_Exhaustion"": null,
            ""OfflineMode"": null,
            ""BadStateNumber"": null,
            ""StrictFormSign"": null,
            ""ServiceSectorSign"": null,
            ""EncryptionSign"": null,
            ""AutoMode"": null,
            ""KKT_MachineNumber"": null,
            ""InternetSign"": null,
            ""OfdInn"": null,
            ""KKT_FactoryNumber"": null,
            ""FiscalDrive_ReplaceRequired"": null,
            ""FiscalDrive_MemoryExceeded"": null,
            ""ReRegReasons"": null,
            ""CheckFP_Site"": null,
            ""PaymentSubAgent_Phone"": null,
            ""PaymentOperator_Phone"": null,
            ""PaymentAgent_Comission"": null,
            ""BankSubAgent_Phone"": null,
            ""BankSubAgent_Operation"": null,
            ""Messages"": null,
            ""DiscountMarkup"": null,
            ""Extra"": null,
            ""Format_Version"": null,
            ""Format_VersionKKT"": null,
            ""Format_VersionFN"": null,
            ""Correction_Type"": null,
            ""Correction"": null,
            ""Amount_Advance"": 0,
            ""Amount_Loan"": 0,
            ""Amount_Granting"": 0,
            ""Supplier_Phone"": null,
            ""TaxAuthority_Site"": null,
            ""AdditionalRequisite"": null,
            ""ShiftTotals"": null,
            ""DeliveredTotals"": null,
            ""UndeliveredTotals"": null,
            ""Calculation_Place"": null,
            ""GamblingMode"": null,
            ""PaymentAgentMode"": null,
            ""LotteryMode"": null,
            ""Sign_KKT_Machine"": null,
            ""Sign_Excise"": null,
            ""Operator_INN"": null,
            ""RecipeSite"": null,
            ""ValidityPeriod"": null
        },
        ""Tag"": 0,
        ""UserInn"": ""7814148471  "",
        ""KktRegNumber"": ""0005850057026850    "",
        ""FnNumber"": ""7380440801197696"",
        ""DocNumber"": 27799,
        ""DocDateTime"": ""2025-10-23T09:30:00"",
        ""DocFiscalSign"": ""AcrQHwAA"",
        ""DecimalFiscalSign"": ""533776897"",
        ""CDateUtc"": ""2025-10-23T09:30:00"",
        ""SchemeWarnings"": null,
        ""ValidationErrors"": null,
        ""RegAddresss"": ""г. Москва, Веласкеса б-р., д. 3, к. 1"",
        ""FiasId"": ""e0221a1f-4c74-45f4-992c-ab8a841620a5"",
        ""GeoPoint"": {
            ""Latitude"": 55.592947,
            ""Longitude"": 37.457358
        }
    },
    ""AvailableRequests"": 141
}";

            return JsonConvert.DeserializeObject<OfdApiResponse>(ofdApiResponseJson)!;
        }
    }
}
