namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с плейсхолдерами в документах
    /// </summary>
    public interface IDocumentPlaceholderService
    {
        /// <summary>
        /// Заменить плейсхолдеры в документе .docx
        /// </summary>
        /// <param name="templateStream">Поток с шаблоном документа</param>
        /// <param name="placeholderData">Данные для замены плейсхолдеров</param>
        /// <returns>Поток с обработанным документом</returns>
        Task<Stream> ReplacePlaceholdersInDocxAsync(Stream templateStream, Dictionary<string, string> placeholderData);

        /// <summary>
        /// Извлечь список плейсхолдеров из документа .docx
        /// </summary>
        /// <param name="templateStream">Поток с шаблоном документа</param>
        /// <returns>Список уникальных плейсхолдеров</returns>
        Task<List<string>> ExtractPlaceholdersFromDocxAsync(Stream templateStream);

        /// <summary>
        /// Конвертировать .docx в .rtf
        /// </summary>
        /// <param name="docxStream">Поток с документом .docx</param>
        /// <returns>Поток с документом .rtf</returns>
        Task<Stream> ConvertDocxToRtfAsync(Stream docxStream);

        /// <summary>
        /// Конвертировать .docx в .pdf
        /// </summary>
        /// <param name="docxStream">Поток с документом .docx</param>
        /// <returns>Поток с документом .pdf</returns>
        Task<Stream> ConvertDocxToPdfAsync(Stream docxStream);
    }
}
