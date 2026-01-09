using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Text.RegularExpressions;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с плейсхолдерами в документах
    /// </summary>
    public class DocumentPlaceholderService : IDocumentPlaceholderService
    {
        #region Логгеры

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<DocumentPlaceholderService> _logger;

        #endregion

        #region Приватные поля

        /// <summary>
        /// Regex для поиска плейсхолдеров в формате {{ключ}}
        /// </summary>
        private static readonly Regex PlaceholderRegex = new(
            @"\{\{([^}]+)\}\}",
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(1));

        #endregion

        /// <summary>
        /// Сервис работы с плейсхолдерами в документах
        /// </summary>
        /// <param name="logger">Логгер</param>
        public DocumentPlaceholderService(ILogger<DocumentPlaceholderService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Заменить плейсхолдеры в документе .docx
        /// </summary>
        public async Task<Stream> ReplacePlaceholdersInDocxAsync(Stream templateStream, Dictionary<string, string> placeholderData)
        {
            try
            {
                // Создаем копию потока в памяти
                var outputStream = new MemoryStream();
                await templateStream.CopyToAsync(outputStream);
                outputStream.Position = 0;

                // Открываем документ для редактирования
                using (var wordDocument = WordprocessingDocument.Open(outputStream, true))
                {
                    var body = wordDocument.MainDocumentPart?.Document?.Body;
                    if (body == null)
                    {
                        throw new InvalidOperationException("Документ не содержит основного раздела");
                    }

                    // Заменяем плейсхолдеры во всех текстовых элементах
                    foreach (var text in body.Descendants<Text>())
                    {
                        if (string.IsNullOrEmpty(text.Text))
                            continue;

                        var originalText = text.Text;
                        var modifiedText = originalText;

                        // Заменяем каждый плейсхолдер на соответствующее значение
                        foreach (var placeholder in placeholderData)
                        {
                            var pattern = $"{{{{{placeholder.Key}}}}}";
                            modifiedText = modifiedText.Replace(pattern, placeholder.Value);
                        }

                        // Если текст изменился, обновляем его
                        if (modifiedText != originalText)
                        {
                            text.Text = modifiedText;
                        }
                    }

                    // Сохраняем изменения
                    wordDocument.MainDocumentPart?.Document.Save();
                }

                outputStream.Position = 0;
                return outputStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при замене плейсхолдеров в документе");
                throw;
            }
        }

        /// <summary>
        /// Извлечь список плейсхолдеров из документа .docx
        /// </summary>
        public async Task<List<string>> ExtractPlaceholdersFromDocxAsync(Stream templateStream)
        {
            try
            {
                var placeholders = new HashSet<string>();

                // Создаем копию потока для чтения
                var memoryStream = new MemoryStream();
                await templateStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (var wordDocument = WordprocessingDocument.Open(memoryStream, false))
                {
                    var body = wordDocument.MainDocumentPart?.Document?.Body;
                    if (body == null)
                    {
                        return new List<string>();
                    }

                    // Извлекаем все текстовые элементы
                    foreach (var text in body.Descendants<Text>())
                    {
                        if (string.IsNullOrEmpty(text.Text))
                            continue;

                        // Ищем все плейсхолдеры в формате {{key}}
                        var matches = PlaceholderRegex.Matches(text.Text);
                        foreach (Match match in matches)
                        {
                            if (match.Groups.Count > 1)
                            {
                                placeholders.Add(match.Groups[1].Value.Trim());
                            }
                        }
                    }
                }

                return placeholders.OrderBy(p => p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при извлечении плейсхолдеров из документа");
                throw;
            }
        }

        /// <summary>
        /// Конвертировать .docx в .rtf
        /// </summary>
        public async Task<Stream> ConvertDocxToRtfAsync(Stream docxStream)
        {
            // TODO: Реализовать конвертацию в RTF
            // Для этого можно использовать библиотеку RtfPipe или другую альтернативу
            // Пока возвращаем NotImplementedException
            await Task.CompletedTask;
            throw new NotImplementedException("Конвертация в RTF будет реализована позже");
        }

        /// <summary>
        /// Конвертировать .docx в .pdf
        /// </summary>
        public async Task<Stream> ConvertDocxToPdfAsync(Stream docxStream)
        {
            // TODO: Реализовать конвертацию в PDF
            // Для этого потребуется дополнительная библиотека, например:
            // - Aspose.Words (платная)
            // - Syncfusion DocIO (бесплатная для community)
            // - LibreOffice через command line
            await Task.CompletedTask;
            throw new NotImplementedException("Конвертация в PDF будет реализована позже");
        }
    }
}
