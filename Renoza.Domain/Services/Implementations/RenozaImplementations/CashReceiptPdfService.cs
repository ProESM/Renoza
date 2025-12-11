using Microsoft.Extensions.Logging;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Renoza.Domain.Entities.CashReceipts.OfdApi;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для генерации PDF версии кассового чека
    /// </summary>
    public class CashReceiptPdfService : ICashReceiptPdfService
    {
        private readonly ILogger<CashReceiptPdfService> _logger;

        public CashReceiptPdfService(ILogger<CashReceiptPdfService> logger)
        {
            _logger = logger;

            // TODO надо оценить, купить лицензию или поискать альтернативу
            // Устанавливаем лицензию QuestPDF (Community License для некоммерческого использования)
            QuestPDF.Settings.License = LicenseType.Community;

            // Отключаем проверку доступности глифов для всех символов
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
        }

        /// <summary>
        /// Генерирует PDF файл чека и возвращает Stream
        /// </summary>
        public async Task<Stream> GeneratePdfAsync(OfdReceiptData receiptData)
        {
            try
            {
                _logger.LogInformation("Начало генерации PDF чека");

                return await Task.Run(() =>
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            // Устанавливаем размер как у чековой ленты (58мм или 80мм ширина)
                            page.Size(226, PageSizes.A4.Height); // 80мм = ~226 points
                            page.Margin(10);
                            page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Roboto"));

                            page.Content().Column(column =>
                            {
                                column.Spacing(2);

                                // Заголовок - как на настоящем чеке (временно без типа операции)
                                column.Item().PaddingVertical(2).BorderBottom(1).BorderColor("#000000");

                                // Информация о продавце
                                if (receiptData.Document != null)
                                {
                                    var doc = receiptData.Document;

                                    // Название организации
                                    if (!string.IsNullOrEmpty(doc.User))
                                    {
                                        column.Item().AlignCenter().Text(doc.User.ToUpper()).FontSize(9).Bold();
                                    }

                                    // ИНН
                                    if (!string.IsNullOrEmpty(doc.UserInn))
                                    {
                                        column.Item().AlignCenter().Text($"ИНН {doc.UserInn}").FontSize(8);
                                    }

                                    // Адрес
                                    if (!string.IsNullOrEmpty(doc.RetailPlaceAddress))
                                    {
                                        column.Item().AlignCenter().Text(doc.RetailPlaceAddress).FontSize(7);
                                    }

                                    column.Item().PaddingVertical(3).BorderBottom(1).BorderColor("#000000");

                                    // Тип операции в заголовке
                                    string operationType = doc.OperationType switch
                                    {
                                        1 => "ПРИХОД",
                                        2 => "ВОЗВРАТ ПРИХОДА",
                                        3 => "РАСХОД",
                                        4 => "ВОЗВРАТ РАСХОДА",
                                        _ => "НЕИЗВЕСТНО"
                                    };
                                    column.Item().AlignCenter().Text($"КАССОВЫЙ ЧЕК / {operationType}").FontSize(10).Bold();

                                    // Дата/время, чек, кассир, смена - в 3 столбца и 2 строки
                                    column.Item().Row(row =>
                                    {
                                        // Левый столбец - дата/время и кассир
                                        row.RelativeItem(2).Column(leftColumn =>
                                        {
                                            leftColumn.Item().Text($"{doc.DateTime:dd.MM.yyyy HH:mm}").FontSize(8);
                                            if (!string.IsNullOrEmpty(doc.Operator))
                                            {
                                                leftColumn.Item().Text($"КАССИР: {doc.Operator}").FontSize(7);
                                            }
                                        });

                                        // Средний столбец - метки "ЧЕК:" и "СМЕНА:" (выравнивание влево)
                                        row.AutoItem().Column(middleColumn =>
                                        {
                                            middleColumn.Item().Text("ЧЕК:").FontSize(8);
                                            middleColumn.Item().Text("СМЕНА:").FontSize(7);
                                        });

                                        // Правый столбец - номера чека и смены (выравнивание вправо)
                                        row.AutoItem().PaddingLeft(5).Column(rightColumn =>
                                        {
                                            rightColumn.Item().AlignRight().Text($"{doc.Number}").FontSize(8);
                                            rightColumn.Item().AlignRight().Text($"{doc.ShiftNumber}").FontSize(7);
                                        });
                                    });

                                    column.Item().PaddingVertical(2).BorderBottom(1).BorderColor("#000000");

                                    int vatValue = doc.DateTime < new DateTime(2019, 1, 1)
                                        ? 18
                                        : doc.DateTime < new DateTime(2026, 1, 1)
                                            ? 20
                                            : 22;

                                    // Таблица товаров
                                    if (doc.Items != null && doc.Items.Any())
                                    {
                                        int itemNumber = 1;
                                        foreach (var item in doc.Items)
                                        {
                                            column.Item().Column(itemColumn =>
                                            {
                                                // Номер и название товара
                                                itemColumn.Item().Text($"{itemNumber}. {item.Name ?? "Без названия"}").FontSize(8);

                                                // Цена x Количество = Сумма (в одну строку как в реальном чеке)
                                                itemColumn.Item().Row(itemRow =>
                                                {
                                                    itemRow.RelativeItem().Text($"    {FormatCurrency(item.Price)} x {FormatQuantity(item.Quantity)} =").FontSize(8);
                                                    itemRow.AutoItem().AlignRight().Text($"{FormatCurrency(item.Total)}").FontSize(8).Bold();
                                                });

                                                // НДС если есть
                                                if (item.NDS_Rate.HasValue)
                                                {
                                                    string ndsRate = item.NDS_Rate.Value switch
                                                    {
                                                        1 => $"НДС {vatValue}%",
                                                        2 => "НДС 10%",
                                                        3 => $"НДС {vatValue}/1{vatValue}",
                                                        4 => "НДС 10/110",
                                                        5 => "НДС 0%",
                                                        6 => "Без НДС",
                                                        _ => $"НДС {item.NDS_Rate.Value}"
                                                    };
                                                    itemColumn.Item().Text($"     {ndsRate}").FontSize(7);
                                                }
                                            });
                                            itemNumber++;
                                        }

                                        column.Item().PaddingVertical(2).BorderBottom(1).BorderColor("#000000");
                                    }

                                    // Итого
                                    column.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("ИТОГО:").Bold().FontSize(10);
                                        row.AutoItem().AlignRight().Text(FormatCurrency(doc.Amount_Total)).Bold().FontSize(10);
                                    });

                                    // Способ оплаты
                                    if (doc.Amount_Cash > 0)
                                    {
                                        column.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text("НАЛИЧНЫМИ").FontSize(8);
                                            row.AutoItem().AlignRight().Text(FormatCurrency(doc.Amount_Cash)).FontSize(8);
                                        });
                                    }
                                    if (doc.Amount_ECash > 0)
                                    {
                                        column.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text("ЭЛЕКТРОННЫМИ").FontSize(8);
                                            row.AutoItem().AlignRight().Text(FormatCurrency(doc.Amount_ECash)).FontSize(8);
                                        });
                                    }

                                    column.Item().PaddingVertical(2).BorderBottom(1).BorderColor("#000000");

                                    // НДС итого
                                    if (doc.NdsNA_TotalSumm > 0)
                                    {
                                        column.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text("БЕЗ НДС").FontSize(8);
                                            row.AutoItem().AlignRight().Text(FormatCurrency(doc.NdsNA_TotalSumm)).FontSize(8);
                                        });
                                    }

                                    if (doc.Nds18_TotalSumm > 0 || doc.Nds10_TotalSumm > 0 || doc.Nds00_TotalSumm > 0)
                                    {
                                        if (doc.Nds18_TotalSumm > 0)
                                        {
                                            column.Item().Row(row =>
                                            {
                                                row.RelativeItem().Text($"НДС {vatValue}%").FontSize(8);
                                                row.AutoItem().AlignRight().Text(FormatCurrency(doc.Nds18_TotalSumm)).FontSize(8);
                                            });
                                        }
                                        if (doc.Nds10_TotalSumm > 0)
                                        {
                                            column.Item().Row(row =>
                                            {
                                                row.RelativeItem().Text("НДС 10%").FontSize(8);
                                                row.AutoItem().AlignRight().Text(FormatCurrency(doc.Nds10_TotalSumm)).FontSize(8);
                                            });
                                        }
                                        if (doc.Nds00_TotalSumm > 0)
                                        {
                                            column.Item().Row(row =>
                                            {
                                                row.RelativeItem().Text("НДС 0%").FontSize(8);
                                                row.AutoItem().AlignRight().Text(FormatCurrency(doc.Nds00_TotalSumm)).FontSize(8);
                                            });
                                        }
                                    }

                                    column.Item().PaddingVertical(3).BorderBottom(1).BorderColor("#000000");

                                    // Фискальные данные - компактно, как на настоящем чеке
                                    column.Item().Text($"РН ККТ:{doc.KKT_RegNumber?.Trim()}").FontSize(7);
                                    column.Item().Text($"ЗН ККТ:{doc.KKT_FactoryNumber}").FontSize(7);
                                    column.Item().Text($"ФН:{doc.FN_FactoryNumber}").FontSize(7);
                                    column.Item().Text($"ФД:{doc.Document_Number}  ФПД:{doc.DecimalFiscalSign}").FontSize(7);

                                    column.Item().PaddingVertical(2).BorderBottom(1).BorderColor("#000000");

                                    // Информация для проверки
                                    column.Item().AlignCenter().Text("СПАСИБО ЗА ПОКУПКУ!").FontSize(8).Bold();
                                    column.Item().PaddingTop(3).AlignCenter().Text("Проверить чек:").FontSize(7);
                                    column.Item().AlignCenter().Text("www.nalog.gov.ru").FontSize(7);

                                    // QR код
                                    var qrData = $"t={receiptData.DocDateTime:yyyyMMddTHHmm}&s={doc.Amount_Total / 100.0m:F2}&fn={receiptData.FnNumber}&i={doc.Document_Number}&fp={doc.DecimalFiscalSign}&n={doc.OperationType}";
                                    var qrCodeImage = GenerateQrCode(qrData);

                                    column.Item().PaddingTop(5).AlignCenter().Width(150).Height(150).Image(qrCodeImage);

                                    column.Item().PaddingTop(5).AlignCenter().Text($"ЧЕК № {doc.Document_Number}").FontSize(7);
                                }
                            });
                        });
                    });

                    var stream = new MemoryStream();
                    document.GeneratePdf(stream);
                    stream.Position = 0; // Сбрасываем позицию на начало для чтения

                    _logger.LogInformation("PDF чек успешно сгенерирован");
                    return stream;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации PDF чека");
                throw;
            }
        }

        /// <summary>
        /// Форматирует сумму в копейках в рубли
        /// </summary>
        private string FormatCurrency(int? amountInKopecks)
        {
            if (!amountInKopecks.HasValue)
                return "0.00";

            decimal rubles = amountInKopecks.Value / 100.0m;
            return $"{rubles:F2}";
        }

        /// <summary>
        /// Форматирует сумму в копейках в рубли (decimal)
        /// </summary>
        private string FormatCurrency(decimal? amountInKopecks)
        {
            if (!amountInKopecks.HasValue)
                return "0.00";

            decimal rubles = amountInKopecks.Value / 100.0m;
            return $"{rubles:F2}";
        }

        /// <summary>
        /// Форматирует количество
        /// </summary>
        private string FormatQuantity(decimal? quantity)
        {
            if (!quantity.HasValue)
                return "0";

            return $"{quantity.Value}";
        }

        /// <summary>
        /// Генерирует QR-код из строки данных
        /// </summary>
        private byte[] GenerateQrCode(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20); // 20 пикселей на модуль
        }
    }
}
