namespace Renoza.Common.Base.Exceptions
{
    /// <summary>
    /// Базовый класс пользовательской ошибки 
    /// </summary>
    public class BaseException : Exception
    {
        public BaseException() { }
        public BaseException(string message) : base(message) { }
        public BaseException(string message, Exception inner) : base(message, inner) { }
    }
}
