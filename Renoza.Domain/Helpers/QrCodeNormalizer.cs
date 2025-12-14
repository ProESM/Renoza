namespace Renoza.Domain.Helpers
{
    /// <summary>
    /// Утилита для нормализации QR кодов кассовых чеков
    /// </summary>
    public static class QrCodeNormalizer
    {
        /// <summary>
        /// Нормализует QR код для сравнения и поиска дубликатов.
        /// Парсит параметры QR кода и формирует каноническую строку с отсортированными параметрами.
        /// Это гарантирует, что QR коды с одинаковыми данными, но разным порядком параметров,
        /// будут иметь одинаковое нормализованное значение.
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

            // 4. Если это URL, извлекаем query string параметры
            if (normalized.StartsWith("http://") || normalized.StartsWith("https://"))
            {
                var queryStart = normalized.IndexOf('?');
                if (queryStart >= 0 && queryStart < normalized.Length - 1)
                {
                    normalized = normalized.Substring(queryStart + 1);
                }
                else
                {
                    // URL без параметров - возвращаем как есть после базовой нормализации
                    return normalized.TrimEnd('/');
                }
            }

            // 5. Парсим параметры и сортируем их для канонической формы
            // Формат: t=yyyyMMddTHHmm&s=сумма&fn=фн&i=фд&fp=фп&n=тип
            var parameters = ParseParameters(normalized);
            if (parameters.Count == 0)
            {
                // Если не удалось распарсить параметры, возвращаем исходную нормализованную строку
                return normalized;
            }

            // 6. Сортируем параметры по ключу и формируем каноническую строку
            var canonicalPairs = parameters
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            return string.Join("&", canonicalPairs);
        }

        /// <summary>
        /// Парсит строку параметров в словарь ключ-значение
        /// </summary>
        private static Dictionary<string, string> ParseParameters(string paramString)
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(paramString))
            {
                return parameters;
            }

            // Разбиваем по символу '&'
            var pairs = paramString.Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var parts = pair.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    if (!string.IsNullOrEmpty(key))
                    {
                        // Если ключ уже существует, берем первое значение (как в стандартных query strings)
                        if (!parameters.ContainsKey(key))
                        {
                            parameters[key] = value;
                        }
                    }
                }
            }

            return parameters;
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
