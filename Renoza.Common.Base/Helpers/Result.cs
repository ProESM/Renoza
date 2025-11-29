namespace Renoza.Common.Helpers
{
    /// <summary>
    /// Результат выполнения операции
    /// </summary>
    /// <typeparam name="T">Тип данных результата</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Успешность выполнения операции
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Сообщение об ошибке (если операция не успешна)
        /// </summary>
        public string? ErrorMessage { get; private set; }

        /// <summary>
        /// Данные результата (если операция успешна)
        /// </summary>
        public T? Data { get; private set; }

        private Result(bool isSuccess, T? data, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Создает успешный результат
        /// </summary>
        /// <param name="data">Данные результата</param>
        /// <returns>Успешный результат</returns>
        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, null);
        }

        /// <summary>
        /// Создает неудачный результат
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <returns>Неудачный результат</returns>
        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T>(false, default, errorMessage);
        }
    }
}
