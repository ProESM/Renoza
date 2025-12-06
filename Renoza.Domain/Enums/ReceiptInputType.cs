namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Тип входных данных чека
    /// </summary>
    public enum ReceiptInputType
    {
        /// <summary>
        /// QR код (строка)
        /// </summary>
        QrCode = 1,

        /// <summary>
        /// Фото (base64)
        /// </summary>
        Photo = 2,

        /// <summary>
        /// Файл изображения (base64)
        /// </summary>
        ImageFile = 3,

        /// <summary>
        /// PDF файл (base64)
        /// </summary>
        PdfFile = 4
    }
}
