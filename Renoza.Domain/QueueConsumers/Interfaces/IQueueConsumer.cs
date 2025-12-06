using MassTransit;

namespace Renoza.Domain.QueueConsumers.Interfaces
{
    /// <summary>
    /// Базовый интерфейс для Consumer-ов сообщений
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IQueueConsumer<in TMessage> : IConsumer<TMessage> where TMessage : class
    {
    }
}
