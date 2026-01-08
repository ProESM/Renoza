namespace Renoza.Domain.Constants.Products
{
    /// <summary>
    /// Идентификаторы типов единиц измерения
    /// </summary>
    public static class MeasurementUnitTypeIds
    {
        /// <summary>
        /// Количество (штучные)
        /// </summary>
        public static readonly Guid Quantity = new("00000000-0000-0000-0000-000000000001");
        /// <summary>
        /// Масса
        /// </summary>
        public static readonly Guid Weight = new("00000000-0000-0000-0000-000000000002");
        /// <summary>
        /// Длина
        /// </summary>
        public static readonly Guid Length = new("00000000-0000-0000-0000-000000000003");
        /// <summary>
        /// Площадь
        /// </summary>
        public static readonly Guid Area = new("00000000-0000-0000-0000-000000000004");
        /// <summary>
        /// Объём
        /// </summary>
        public static readonly Guid Volume = new("00000000-0000-0000-0000-000000000005");
        /// <summary>
        /// Время
        /// </summary>
        public static readonly Guid Time = new("00000000-0000-0000-0000-000000000006");
        /// <summary>
        /// Проценты и доли
        /// </summary>
        public static readonly Guid Ratio = new("00000000-0000-0000-0000-000000000007");
        /// <summary>
        /// Энергия / мощность
        /// </summary>
        public static readonly Guid Energy = new("00000000-0000-0000-0000-000000000008");
    }
}
