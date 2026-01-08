namespace Renoza.Domain.Constants.Company
{
    /// <summary>
    /// Идентификаторы ролей участников компании
    /// </summary>
    public static class CompanyMemberRoleIds
    {
        /// <summary>
        /// Владелец - полный контроль над компанией
        /// </summary>
        public static readonly Guid Owner = new Guid("10000000-0000-0000-0004-000000000001");

        /// <summary>
        /// Менеджер - управление заказами и сотрудниками
        /// </summary>
        public static readonly Guid Manager = new Guid("10000000-0000-0000-0004-000000000002");

        /// <summary>
        /// Сотрудник - базовые права на работу
        /// </summary>
        public static readonly Guid Employee = new Guid("10000000-0000-0000-0004-000000000003");
    }
}
