using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Domain.Entities.CompanyVerification.DaDataApi;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.CompanyVerification;
using Renoza.Domain.Options;
using Renoza.Domain.QueueConsumers.Interfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Domain.QueueConsumers.Implementations.CompanyVerificationBroker
{
    /// <summary>
    /// Consumer для обработки верификации компании через DaData API
    /// </summary>
    public class CompanyVerificationProcessingConsumer : IQueueConsumer<CompanyVerificationProcessingMessage>
    {
        /// <summary>
        /// Логгер для записи информации о процессе обработки верификации
        /// </summary>
        private readonly ILogger<CompanyVerificationProcessingConsumer> _logger;
        /// <summary>
        /// Шина сообщений MassTransit для отправки сообщений в очередь
        /// </summary>
        private readonly IBus _bus;
        /// <summary>
        /// Настройки брокера верификации компаний (имена очередей, настройки API)
        /// </summary>
        private readonly CompanyVerificationBrokerOptions _companyVerificationBrokerOptions;
        /// <summary>
        /// Сервис для управления заданиями на верификацию компаний
        /// </summary>
        private readonly ICompanyVerificationJobService _companyVerificationJobService;
        /// <summary>
        /// Сервис для работы с DaData API (получение информации о компаниях по ИНН)
        /// </summary>
        private readonly IDaDataApiService _daDataApiService;

        /// <summary>
        /// Consumer для обработки верификации компании через DaData API
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="bus">Шина сообщений MassTransit для отправки сообщений в очередь</param>
        /// <param name="companyVerificationBrokerOptions">Настройки брокера верификации компаний</param>
        /// <param name="companyVerificationJobService">Сервис для управления заданиями на верификацию компаний</param>
        /// <param name="daDataApiService">Сервис для работы с DaData API</param>
        public CompanyVerificationProcessingConsumer(
            ILogger<CompanyVerificationProcessingConsumer> logger,
            IBus bus,
            CompanyVerificationBrokerOptions companyVerificationBrokerOptions,
            ICompanyVerificationJobService companyVerificationJobService,
            IDaDataApiService daDataApiService)
        {
            _logger = logger;
            _bus = bus;
            _companyVerificationBrokerOptions = companyVerificationBrokerOptions;
            _companyVerificationJobService = companyVerificationJobService;
            _daDataApiService = daDataApiService;
        }

        public async Task Consume(ConsumeContext<CompanyVerificationProcessingMessage> context)
        {
            await ProcessMessage(context.Message);
        }

        private async Task ProcessMessage(CompanyVerificationProcessingMessage message)
        {
            _logger.LogInformation("Начата обработка верификации компании. JobId: {JobId}, INN: {Inn}",
                message.JobId, message.Inn);

            try
            {
                // Обновляем статус на Processing
                await _companyVerificationJobService.UpdateJobStatusAsync(
                    message.JobId,
                    (short)CompanyVerificationJobStatus.Processing,
                    "Запрос данных компании через DaData API");

                // Запрос к DaData API
                _logger.LogInformation("Запрос информации о компании через DaData API. INN: {Inn}", message.Inn);
                //var response = await _daDataApiService.GetCompanyByInnAsync(message.Inn);

                // TODO: Временно замокаем данные, но нужно будет убрать, когда подключим внешний сервис
                var response = GetMockDaDataApiResponse(message.Inn);

                if (response == null || response.Suggestions == null || !response.Suggestions.Any())
                {
                    _logger.LogWarning("Компания не найдена в DaData. INN: {Inn}", message.Inn);

                    await _companyVerificationJobService.UpdateJobStatusAsync(
                        message.JobId,
                        (short)CompanyVerificationJobStatus.CompanyNotFound,
                        "Компания не найдена в базе данных DaData");

                    return;
                }

                var companyData = response.Suggestions.First().Data;
                var isActive = companyData.State?.Status == "ACTIVE";

                // Обновляем статус на DataReceived
                await _companyVerificationJobService.UpdateJobStatusAsync(
                    message.JobId,
                    (short)CompanyVerificationJobStatus.DataReceived,
                    "Данные о компании получены от внешнего сервиса");

                _logger.LogInformation("Получена информация о компании. INN: {Inn}, Name: {Name}, IsActive: {IsActive}",
                    message.Inn, companyData.Name?.ShortWithOpf, isActive);

                // Обновляем статус на Saving
                await _companyVerificationJobService.UpdateJobStatusAsync(
                    message.JobId,
                    (short)CompanyVerificationJobStatus.Saving,
                    "Сохранение результатов верификации");

                // Отправляем на сохранение
                var companyVerificationSaveMessage = new CompanyVerificationSaveMessage
                {
                    JobId = message.JobId,
                    CompanyProfileId = message.CompanyProfileId,
                    Inn = message.Inn,
                    CompanyType = companyData.Type,
                    Name = companyData.Name?.ShortWithOpf,
                    FullName = companyData.Name?.FullWithOpf,
                    Ogrn = companyData.Ogrn,
                    Kpp = companyData.Kpp,
                    Address = companyData.Address?.UnrestrictedValue,
                    DirectorName = companyData.Management != null 
                        ? companyData.Management.Name 
                        : string.IsNullOrWhiteSpace(companyData.Type) 
                            ? null 
                            : companyData.Type.Equals("INDIVIDUAL", StringComparison.InvariantCultureIgnoreCase) 
                                ? companyData.Name?.Full
                                : null,
                    RegistrationDate = companyData.State?.RegistrationDate.HasValue == true ? DateTimeOffset.FromUnixTimeMilliseconds(companyData.State.RegistrationDate.Value).DateTime : null,
                    IsActive = isActive,
                    JsonData = JsonConvert.SerializeObject(response)
                };

                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_companyVerificationBrokerOptions.CompanyVerificationSaveConsumerQueueName}"));
                await endpoint.Send(companyVerificationSaveMessage);

                _logger.LogInformation("JobId: {JobId}: Отправлено на сохранение", message.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке верификации. JobId: {JobId}, INN: {Inn}",
                    message.JobId, message.Inn);

                await _companyVerificationJobService.UpdateJobStatusAsync(
                    message.JobId,
                    (short)CompanyVerificationJobStatus.ExternalServiceFailed,
                    $"Внутренняя ошибка: {ex.Message}");

                throw;
            }
        }

        private DaDataCompanyResponse? GetMockDaDataApiResponse(string inn)
        {
            var companyDictionary = new Dictionary<string, string>();

            companyDictionary.Add("7719402047", @"{
  ""suggestions"": [
    {
      ""value"": ""ООО \""МОТОРИКА\"""",
      ""unrestricted_value"": ""ООО \""МОТОРИКА\"""",
      ""data"": {
        ""kpp"": ""772301001"",
        ""kpp_largest"": null,
        ""capital"": {
          ""type"": ""УСТАВНЫЙ КАПИТАЛ"",
          ""value"": 64683.7
        },
        ""invalid"": null,
        ""management"": {
          ""name"": ""Давидюк Андрей Павлович"",
          ""post"": ""ГЕНЕРАЛЬНЫЙ ДИРЕКТОР"",
          ""start_date"": 1669237200000,
          ""disqualified"": null
        },
        ""founders"": [
          {
            ""ogrn"": ""1232500002140"",
            ""inn"": ""2540274313"",
            ""name"": ""МЕЖДУНАРОДНАЯ КОМПАНИЯ ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ \""ХОМО АУКТУС\"""",
            ""hid"": ""162fda77a319e6654cb77fc9e70593f07e73c4a447051c20a742978dc546d2f1"",
            ""type"": ""LEGAL"",
            ""share"": {
              ""value"": 100,
              ""type"": ""PERCENT""
            },
            ""invalidity"": null,
            ""start_date"": 1676840400000
          }
        ],
        ""managers"": [
          {
            ""inn"": ""782617173381"",
            ""fio"": {
              ""surname"": ""Давидюк"",
              ""name"": ""Андрей"",
              ""patronymic"": ""Павлович"",
              ""gender"": ""MALE"",
              ""source"": ""ДАВИДЮК АНДРЕЙ ПАВЛОВИЧ"",
              ""qc"": null
            },
            ""post"": ""ГЕНЕРАЛЬНЫЙ ДИРЕКТОР"",
            ""hid"": ""88fe9c0a7f8d26948d2e5305b442a83c367f1029b9420fd720d196244f1d9ec3"",
            ""type"": ""EMPLOYEE"",
            ""invalidity"": null,
            ""start_date"": 1669237200000
          }
        ],
        ""predecessors"": null,
        ""successors"": null,
        ""branch_type"": ""MAIN"",
        ""branch_count"": 0,
        ""source"": null,
        ""qc"": null,
        ""hid"": ""baf582914d601bc5246e881b07dfa6e336091a3857bebc3bf389aa0b4073223c"",
        ""type"": ""LEGAL"",
        ""state"": {
          ""status"": ""ACTIVE"",
          ""code"": null,
          ""actuality_date"": 1732060800000,
          ""registration_date"": 1423094400000,
          ""liquidation_date"": null
        },
        ""opf"": {
          ""type"": ""2014"",
          ""code"": ""12300"",
          ""full"": ""Общество с ограниченной ответственностью"",
          ""short"": ""ООО""
        },
        ""name"": {
          ""full_with_opf"": ""ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ \""МОТОРИКА\"""",
          ""short_with_opf"": ""ООО \""МОТОРИКА\"""",
          ""latin"": null,
          ""full"": ""МОТОРИКА"",
          ""short"": ""МОТОРИКА""
        },
        ""inn"": ""7719402047"",
        ""ogrn"": ""1157746078984"",
        ""okpo"": ""27539247"",
        ""okato"": ""45290582000"",
        ""oktmo"": ""45393000000"",
        ""okogu"": ""4210014"",
        ""okfs"": ""16"",
        ""okved"": ""72.19"",
        ""okveds"": [
          {
            ""main"": true,
            ""type"": ""2014"",
            ""code"": ""72.19"",
            ""name"": ""Научные исследования и разработки в области естественных и технических наук прочие""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""24.34"",
            ""name"": ""Производство проволоки методом холодного волочения""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""24.41"",
            ""name"": ""Производство драгоценных металлов""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""25.61"",
            ""name"": ""Обработка металлов и нанесение покрытий на металлы""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""26.60.4"",
            ""name"": ""Производство инструмента, оборудования и приспособлений, применяемых в медицинских целях""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""26.60.5"",
            ""name"": ""Производство диагностического и терапевтического оборудования, применяемого в медицинских целях""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""26.60.9"",
            ""name"": ""Производство прочего оборудования, применяемого в медицинских целях""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""32.12.1"",
            ""name"": ""Производство изделий технического назначения из драгоценных металлов""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""32.50"",
            ""name"": ""Производство медицинских инструментов и оборудования""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""32.99.9"",
            ""name"": ""Производство прочих изделий, не включенных в другие группировки""
          },
          {
            ""main"": false,
            ""type"": ""2014"",
            ""code"": ""38.32.2"",
            ""name"": ""Обработка (переработка) лома и отходов драгоценных металлов""
          }
        ],
        ""authorities"": {
          ""fts_registration"": {
            ""type"": ""FEDERAL_TAX_SERVICE"",
            ""code"": ""7746"",
            ""name"": ""Межрайонная инспекция Федеральной налоговой службы № 46 по г. Москве"",
            ""address"": ""125373, г.Москва, Походный проезд, домовладение 3, стр.2""
          },
          ""fts_report"": {
            ""type"": ""FEDERAL_TAX_SERVICE"",
            ""code"": ""7723"",
            ""name"": ""Инспекция Федеральной налоговой службы № 23 по г.Москве"",
            ""address"": null
          },
          ""pf"": {
            ""type"": ""PENSION_FUND"",
            ""code"": ""087505"",
            ""name"": ""Отделение Фонда пенсионного и социального страхования Российской Федерации по г. Москве и Московской области"",
            ""address"": null
          },
          ""sif"": {
            ""type"": ""SOCIAL_INSURANCE_FUND"",
            ""code"": ""7738"",
            ""name"": ""Отделение Фонда пенсионного и социального страхования Российской Федерации по г. Москве и Московской области"",
            ""address"": null
          }
        },
        ""documents"": {
          ""fts_registration"": {
            ""type"": ""FTS_REGISTRATION"",
            ""series"": ""77"",
            ""number"": ""016942308"",
            ""issue_date"": 1423094400000,
            ""issue_authority"": ""7746""
          },
          ""fts_report"": {
            ""type"": ""FTS_REPORT"",
            ""series"": null,
            ""number"": null,
            ""issue_date"": 1705276800000,
            ""issue_authority"": ""7723""
          },
          ""pf_registration"": {
            ""type"": ""PF_REGISTRATION"",
            ""series"": null,
            ""number"": ""087505029773"",
            ""issue_date"": 1705449600000,
            ""issue_authority"": ""087505""
          },
          ""sif_registration"": {
            ""type"": ""SIF_REGISTRATION"",
            ""series"": null,
            ""number"": ""772406613777381"",
            ""issue_date"": 1512432000000,
            ""issue_authority"": ""7738""
          },
          ""smb"": {
            ""category"": ""SMALL"",
            ""type"": ""SMB"",
            ""series"": null,
            ""number"": null,
            ""issue_date"": 1691625600000,
            ""issue_authority"": null
          }
        },
        ""licenses"": null,
        ""finance"": {
          ""tax_system"": null,
          ""income"": 1895091000,
          ""expense"": 1247123000,
          ""revenue"": 1858707000,
          ""debt"": null,
          ""penalty"": null,
          ""year"": 2023
        },
        ""address"": {
          ""value"": ""г Москва, Волгоградский пр-кт, д 42 к 5, помещ 1Н"",
          ""unrestricted_value"": ""109316, г Москва, р-н Печатники, Волгоградский пр-кт, д 42 к 5, помещ 1Н"",
          ""invalidity"": null,
          ""data"": {
            ""postal_code"": ""109316"",
            ""country"": ""Россия"",
            ""country_iso_code"": ""RU"",
            ""federal_district"": ""Центральный"",
            ""region_fias_id"": ""0c5b2444-70a0-4932-980c-b4dc0d3f02b5"",
            ""region_kladr_id"": ""7700000000000"",
            ""region_iso_code"": ""RU-MOW"",
            ""region_with_type"": ""г Москва"",
            ""region_type"": ""г"",
            ""region_type_full"": ""город"",
            ""region"": ""Москва"",
            ""area_fias_id"": null,
            ""area_kladr_id"": null,
            ""area_with_type"": null,
            ""area_type"": null,
            ""area_type_full"": null,
            ""area"": null,
            ""city_fias_id"": ""0c5b2444-70a0-4932-980c-b4dc0d3f02b5"",
            ""city_kladr_id"": ""7700000000000"",
            ""city_with_type"": ""г Москва"",
            ""city_type"": ""г"",
            ""city_type_full"": ""город"",
            ""city"": ""Москва"",
            ""city_area"": ""Юго-восточный"",
            ""city_district_fias_id"": null,
            ""city_district_kladr_id"": null,
            ""city_district_with_type"": ""р-н Печатники"",
            ""city_district_type"": ""р-н"",
            ""city_district_type_full"": ""район"",
            ""city_district"": ""Печатники"",
            ""settlement_fias_id"": null,
            ""settlement_kladr_id"": null,
            ""settlement_with_type"": null,
            ""settlement_type"": null,
            ""settlement_type_full"": null,
            ""settlement"": null,
            ""street_fias_id"": ""3fbbba41-86cb-4779-9631-e96dd73f536a"",
            ""street_kladr_id"": ""77000000000001300"",
            ""street_with_type"": ""Волгоградский пр-кт"",
            ""street_type"": ""пр-кт"",
            ""street_type_full"": ""проспект"",
            ""street"": ""Волгоградский"",
            ""stead_fias_id"": null,
            ""stead_cadnum"": null,
            ""stead_type"": null,
            ""stead_type_full"": null,
            ""stead"": null,
            ""house_fias_id"": ""7ca4e0cb-83c6-4dad-ac90-9207a90cba02"",
            ""house_kladr_id"": ""7700000000000130276"",
            ""house_cadnum"": ""77:04:0003004:1088"",
            ""house_flat_count"": null,
            ""house_type"": ""д"",
            ""house_type_full"": ""дом"",
            ""house"": ""42"",
            ""block_type"": ""к"",
            ""block_type_full"": ""корпус"",
            ""block"": ""5"",
            ""entrance"": null,
            ""floor"": null,
            ""flat_fias_id"": null,
            ""flat_cadnum"": null,
            ""flat_type"": ""помещ"",
            ""flat_type_full"": ""помещение"",
            ""flat"": ""1Н"",
            ""flat_area"": ""-1"",
            ""square_meter_price"": ""-1"",
            ""flat_price"": null,
            ""room_fias_id"": null,
            ""room_cadnum"": null,
            ""room_type"": null,
            ""room_type_full"": null,
            ""room"": null,
            ""postal_box"": null,
            ""fias_id"": ""7ca4e0cb-83c6-4dad-ac90-9207a90cba02"",
            ""fias_code"": ""77000000000000000130276"",
            ""fias_level"": ""8"",
            ""fias_actuality_state"": ""0"",
            ""kladr_id"": ""7700000000000130276"",
            ""geoname_id"": ""524901"",
            ""capital_marker"": ""0"",
            ""okato"": ""45290582000"",
            ""oktmo"": ""45393000"",
            ""tax_office"": ""7723"",
            ""tax_office_legal"": ""7723"",
            ""timezone"": ""UTC+3"",
            ""geo_lat"": ""55.708875"",
            ""geo_lon"": ""37.721055"",
            ""beltway_hit"": ""IN_MKAD"",
            ""beltway_distance"": null,
            ""metro"": [
              {
                ""name"": ""Текстильщики"",
                ""line"": ""Большая кольцевая линия"",
                ""distance"": 0.5
              },
              {
                ""name"": ""Текстильщики"",
                ""line"": ""Курско-Рижский"",
                ""distance"": 0.6
              },
              {
                ""name"": ""Текстильщики"",
                ""line"": ""Таганско-Краснопресненская"",
                ""distance"": 0.7
              }
            ],
            ""divisions"": null,
            ""qc_geo"": ""0"",
            ""qc_complete"": null,
            ""qc_house"": null,
            ""history_values"": null,
            ""unparsed_parts"": null,
            ""source"": ""109316, Г.МОСКВА, ВН.ТЕР.Г. МУНИЦИПАЛЬНЫЙ ОКРУГ ПЕЧАТНИКИ, ПР-КТ ВОЛГОГРАДСКИЙ, Д. 42, К. 5, ПОМЕЩ. 1Н"",
            ""qc"": ""0""
          }
        },
        ""phones"": [
          {
            ""value"": ""+7 911 2410309"",
            ""unrestricted_value"": ""+7 911 2410309"",
            ""data"": {
              ""contact"": null,
              ""source"": ""+7 911 241 0309"",
              ""qc"": null,
              ""type"": ""Мобильный"",
              ""number"": ""2410309"",
              ""extension"": null,
              ""provider"": ""ПАО \""Мобильные ТелеСистемы\"""",
              ""country"": null,
              ""region"": ""Санкт-Петербург и Ленинградская область"",
              ""city"": null,
              ""timezone"": ""UTC+3"",
              ""country_code"": ""7"",
              ""city_code"": ""911"",
              ""qc_conflict"": null
            }
          }
        ],
        ""emails"": [
          {
            ""value"": ""DAP@MOTORICA.ORG"",
            ""unrestricted_value"": ""DAP@MOTORICA.ORG"",
            ""data"": {
              ""local"": ""DAP"",
              ""domain"": ""MOTORICA.ORG"",
              ""type"": null,
              ""source"": ""DAP@MOTORICA.ORG"",
              ""qc"": null
            }
          }
        ],
        ""ogrn_date"": 1423094400000,
        ""okved_type"": ""2014"",
        ""employee_count"": 111
      }
    }
  ]
}");
            companyDictionary.Add("503120661820", @"{
    ""suggestions"": [
        {
            ""value"": ""ИП Матлашов Петр Егорович"",
            ""unrestricted_value"": ""ИП Матлашов Петр Егорович"",
            ""data"": {
                ""citizenship"": null,
                ""fio"": {
                    ""surname"": ""Матлашов"",
                    ""name"": ""Петр"",
                    ""patronymic"": ""Егорович"",
                    ""gender"": null,
                    ""source"": null,
                    ""qc"": null
                },
                ""source"": null,
                ""qc"": null,
                ""hid"": ""e9766c9f8cb1a918eec2a4eb3db4d21765d2ffbf216a95ea8cc987754fc34d86"",
                ""type"": ""INDIVIDUAL"",
                ""state"": {
                    ""status"": ""ACTIVE"",
                    ""code"": null,
                    ""actuality_date"": 1739923200000,
                    ""registration_date"": 1643673600000,
                    ""liquidation_date"": null
                },
                ""opf"": {
                    ""type"": ""2014"",
                    ""code"": ""50102"",
                    ""full"": ""Индивидуальный предприниматель"",
                    ""short"": ""ИП""
                },
                ""name"": {
                    ""full_with_opf"": ""Индивидуальный предприниматель Матлашов Петр Егорович"",
                    ""short_with_opf"": ""ИП Матлашов Петр Егорович"",
                    ""latin"": null,
                    ""full"": ""Матлашов Петр Егорович"",
                    ""short"": null
                },
                ""inn"": ""503120661820"",
                ""ogrn"": ""322508100056602"",
                ""okpo"": ""2013678266"",
                ""okato"": ""40270000000"",
                ""oktmo"": ""40325000000"",
                ""okogu"": ""4210015"",
                ""okfs"": ""16"",
                ""okved"": ""62.01"",
                ""okveds"": null,
                ""authorities"": null,
                ""documents"": null,
                ""licenses"": null,
                ""finance"": null,
                ""address"": {
                    ""value"": ""г Санкт-Петербург, Приморский пр-кт"",
                    ""unrestricted_value"": ""г Санкт-Петербург, Приморский пр-кт"",
                    ""invalidity"": null,
                    ""data"": {
                        ""postal_code"": null,
                        ""country"": ""Россия"",
                        ""country_iso_code"": ""RU"",
                        ""federal_district"": ""Северо-Западный"",
                        ""region_fias_id"": ""c2deb16a-0330-4f05-821f-1d09c93331e6"",
                        ""region_kladr_id"": ""7800000000000"",
                        ""region_iso_code"": ""RU-SPE"",
                        ""region_with_type"": ""г Санкт-Петербург"",
                        ""region_type"": ""г"",
                        ""region_type_full"": ""город"",
                        ""region"": ""Санкт-Петербург"",
                        ""area_fias_id"": null,
                        ""area_kladr_id"": null,
                        ""area_with_type"": null,
                        ""area_type"": null,
                        ""area_type_full"": null,
                        ""area"": null,
                        ""city_fias_id"": ""c2deb16a-0330-4f05-821f-1d09c93331e6"",
                        ""city_kladr_id"": ""7800000000000"",
                        ""city_with_type"": ""г Санкт-Петербург"",
                        ""city_type"": ""г"",
                        ""city_type_full"": ""город"",
                        ""city"": ""Санкт-Петербург"",
                        ""city_area"": null,
                        ""city_district_fias_id"": null,
                        ""city_district_kladr_id"": null,
                        ""city_district_with_type"": null,
                        ""city_district_type"": null,
                        ""city_district_type_full"": null,
                        ""city_district"": null,
                        ""settlement_fias_id"": null,
                        ""settlement_kladr_id"": null,
                        ""settlement_with_type"": null,
                        ""settlement_type"": null,
                        ""settlement_type_full"": null,
                        ""settlement"": null,
                        ""street_fias_id"": ""2afddf57-f259-48c4-b6ce-078e1748806e"",
                        ""street_kladr_id"": ""78000000000113600"",
                        ""street_with_type"": ""Приморский пр-кт"",
                        ""street_type"": ""пр-кт"",
                        ""street_type_full"": ""проспект"",
                        ""street"": ""Приморский"",
                        ""stead_fias_id"": null,
                        ""stead_cadnum"": null,
                        ""stead_type"": null,
                        ""stead_type_full"": null,
                        ""stead"": null,
                        ""house_fias_id"": null,
                        ""house_kladr_id"": null,
                        ""house_cadnum"": null,
                        ""house_flat_count"": null,
                        ""house_type"": null,
                        ""house_type_full"": null,
                        ""house"": null,
                        ""block_type"": null,
                        ""block_type_full"": null,
                        ""block"": null,
                        ""entrance"": null,
                        ""floor"": null,
                        ""flat_fias_id"": null,
                        ""flat_cadnum"": null,
                        ""flat_type"": null,
                        ""flat_type_full"": null,
                        ""flat"": null,
                        ""flat_area"": null,
                        ""square_meter_price"": null,
                        ""flat_price"": null,
                        ""room_fias_id"": null,
                        ""room_cadnum"": null,
                        ""room_type"": null,
                        ""room_type_full"": null,
                        ""room"": null,
                        ""postal_box"": null,
                        ""fias_id"": ""2afddf57-f259-48c4-b6ce-078e1748806e"",
                        ""fias_code"": ""78000000000000011360000"",
                        ""fias_level"": ""7"",
                        ""fias_actuality_state"": ""0"",
                        ""kladr_id"": ""78000000000113600"",
                        ""geoname_id"": ""498817"",
                        ""capital_marker"": ""0"",
                        ""okato"": ""40000000000"",
                        ""oktmo"": ""40000000"",
                        ""tax_office"": ""7814"",
                        ""tax_office_legal"": ""7814"",
                        ""timezone"": ""UTC+3"",
                        ""geo_lat"": ""59.983711"",
                        ""geo_lon"": ""30.245009"",
                        ""beltway_hit"": ""IN_KAD"",
                        ""beltway_distance"": null,
                        ""metro"": null,
                        ""divisions"": null,
                        ""qc_geo"": ""2"",
                        ""qc_complete"": null,
                        ""qc_house"": null,
                        ""history_values"": null,
                        ""unparsed_parts"": null,
                        ""source"": ""Город Санкт-Петербург город федерального значения, Приморский"",
                        ""qc"": ""0""
                    }
                },
                ""phones"": null,
                ""emails"": null,
                ""ogrn_date"": 1643673600000,
                ""okved_type"": ""2014"",
                ""employee_count"": null
            }
        }
    ]
}");

            if (!companyDictionary.TryGetValue(inn, out var daDataCompanyResponseJson))
            {
                daDataCompanyResponseJson = companyDictionary.Values.First();
            }

            return JsonConvert.DeserializeObject<DaDataCompanyResponse>(daDataCompanyResponseJson)!;
        }
    }
}
