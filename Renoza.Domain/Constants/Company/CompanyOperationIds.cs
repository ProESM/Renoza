namespace Renoza.Domain.Constants.Company
{
    /// <summary>
    /// Идентификаторы операций компании
    /// </summary>
    public static class CompanyOperationIds
    {
        /// <summary>
        /// Создание
        /// </summary>
        public static readonly Guid Create = new Guid("10000000-0000-0000-0001-000000000001");

        /// <summary>
        /// Чтение
        /// </summary>
        public static readonly Guid Read = new Guid("10000000-0000-0000-0001-000000000002");

        /// <summary>
        /// Изменение
        /// </summary>
        public static readonly Guid Update = new Guid("10000000-0000-0000-0001-000000000003");

        /// <summary>
        /// Удаление
        /// </summary>
        public static readonly Guid Delete = new Guid("10000000-0000-0000-0001-000000000004");

        /// <summary>
        /// Выполнение
        /// </summary>
        public static readonly Guid Execute = new Guid("10000000-0000-0000-0001-000000000005");

        /// <summary>
        /// Утверждение
        /// </summary>
        public static readonly Guid Approve = new Guid("10000000-0000-0000-0001-000000000006");

        /// <summary>
        /// Отклонение
        /// </summary>
        public static readonly Guid Reject = new Guid("10000000-0000-0000-0001-000000000007");

        /// <summary>
        /// Экспорт
        /// </summary>
        public static readonly Guid Export = new Guid("10000000-0000-0000-0001-000000000008");

        /// <summary>
        /// Импорт
        /// </summary>
        public static readonly Guid Import = new Guid("10000000-0000-0000-0001-000000000009");

        /// <summary>
        /// Приглашение
        /// </summary>
        public static readonly Guid Invite = new Guid("10000000-0000-0000-0001-000000000010");

        /// <summary>
        /// Удаление участника
        /// </summary>
        public static readonly Guid Remove = new Guid("10000000-0000-0000-0001-000000000011");

        /// <summary>
        /// Назначение
        /// </summary>
        public static readonly Guid Assign = new Guid("10000000-0000-0000-0001-000000000012");
    }
}
