using Renoza.Domain.Entities.CashReceipts.OfdApi;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса для генерации PDF версии кассового чека
    /// </summary>
    public interface ICashReceiptPdfService
    {
        /// <summary>
        /// Генерирует PDF файл чека и возвращает Stream
        /// </summary>
        /// <param name="receiptData">Данные чека от OFD API</param>
        /// <returns>Stream с PDF документом</returns>
        Task<Stream> GeneratePdfAsync(OfdReceiptData receiptData);
    }
}
