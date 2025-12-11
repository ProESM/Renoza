namespace Renoza.Domain.Helpers
{
    /// <summary>
    /// Утилита для нормализации QR кодов кассовых чеков
    /// </summary>
    public static class QrCodeNormalizer
    {
        /// <summary>
        /// Нормализует QR код для сравнения и поиска дубликатов
        /// </summary>
        /// <param name="qrSource">Исходный QR код</param>
        /// <returns>Нормализованный QR код</returns>
        public static string Normalize(string qrSource)
        {
            if (string.IsNullOrWhiteSpace(qrSource))
            {
                return string.Empty;
            }

            // 1. Удаляем пробелы в начале и конце
            var normalized = qrSource.Trim();

            // 2. Приводим к нижнему регистру для case-insensitive сравнения
            normalized = normalized.ToLowerInvariant();

            // 3. Удаляем все пробельные символы (пробелы, табуляции, переносы строк)
            normalized = new string(normalized.Where(c => !char.IsWhiteSpace(c)).ToArray());

            // 4. Если это URL, нормализуем протокол (http/https не важен для содержимого чека)
            if (normalized.StartsWith("http://"))
            {
                normalized = "https://" + normalized.Substring(7);
            }

            // 5. Удаляем завершающий слеш в URL
            if (normalized.EndsWith("/"))
            {
                normalized = normalized.TrimEnd('/');
            }

            return normalized;
        }

        /// <summary>
        /// Сравнивает два QR кода с учетом нормализации
        /// </summary>
        /// <param name="qrSource1">Первый QR код</param>
        /// <param name="qrSource2">Второй QR код</param>
        /// <returns>True, если QR коды эквивалентны после нормализации</returns>
        public static bool AreEqual(string qrSource1, string qrSource2)
        {
            return Normalize(qrSource1) == Normalize(qrSource2);
        }
    }
}
