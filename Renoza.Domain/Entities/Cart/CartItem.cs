namespace Renoza.Domain.Entities.Cart
{
    /// <summary>
    /// Элемент корзины - товар/услуга от конкретного исполнителя
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Идентификатор элемента корзины
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID пользователя (заказчика), владельца корзины
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID профиля исполнителя (Profile - базовая таблица для TPT)
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// ID товара/услуги
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Количество (может быть дробным: 2.5 кг, 15.75 м², 8.5 часов и т.д.)
        /// </summary>
        public decimal Quantity { get; set; } = 1;

        /// <summary>
        /// Комментарий или дополнительные требования к товару/услуге
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
    }
}
