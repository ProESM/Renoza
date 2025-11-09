namespace Renoza.Common.Base.Entities
{
    /// <summary>
    /// Базовый интерфейс для всех сущностей с идентификатором
    /// </summary>
    public interface IEntityWithId<TKey> : IEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        TKey Id { get; set; }
    }
}
