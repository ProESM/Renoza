using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность элемента корзины покупок
    /// </summary>
    public class CartItemDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// ID пользователя (заказчика)
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID профиля исполнителя (технадзор или ремонтная бригада)
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// ID товара/услуги
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Количество (может быть дробным: 2.5 кг, 15.75 м²)
        /// </summary>
        public decimal Quantity { get; set; } = 1;

        /// <summary>
        /// Примечания к заказу
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Дата добавления в корзину
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к пользователю
        /// </summary>
        public virtual UserDao User { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к товару/услуге
        /// </summary>
        public virtual ProductDao Product { get; set; } = null!;
    }
}
