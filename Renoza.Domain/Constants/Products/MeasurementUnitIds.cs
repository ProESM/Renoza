namespace Renoza.Domain.Constants.Products
{
    /// <summary>
    /// Идентификаторы единиц измерения
    /// </summary>
    public static class MeasurementUnitIds
    {
        #region Количество

        /// <summary>
        /// Штука
        /// </summary>
        public static readonly Guid Piece = new("00000000-0000-0000-0000-000000000001");

        /// <summary>
        /// Комплект
        /// </summary>
        public static readonly Guid Set = new("00000000-0000-0000-0000-000000000002");

        /// <summary>
        /// Упаковка
        /// </summary>
        public static readonly Guid Package = new("00000000-0000-0000-0000-000000000003");

        /// <summary>
        /// Пара
        /// </summary>
        public static readonly Guid Pair = new("00000000-0000-0000-0000-000000000004");

        #endregion

        #region Масса

        /// <summary>
        /// Грамм
        /// </summary>
        public static readonly Guid Gram = new("00000000-0000-0000-0000-000000000005");

        /// <summary>
        /// Килограмм
        /// </summary>
        public static readonly Guid Kilogram = new("00000000-0000-0000-0000-000000000006");

        /// <summary>
        /// Тонна
        /// </summary>
        public static readonly Guid Tonne = new("00000000-0000-0000-0000-000000000007");

        #endregion

        #region Длина

        /// <summary>
        /// Миллиметр
        /// </summary>
        public static readonly Guid Millimeter = new("00000000-0000-0000-0000-000000000008");

        /// <summary>
        /// Сантиметр
        /// </summary>
        public static readonly Guid Centimeter = new("00000000-0000-0000-0000-000000000009");

        /// <summary>
        /// Метр
        /// </summary>
        public static readonly Guid Meter = new("00000000-0000-0000-0000-000000000010");

        /// <summary>
        /// Километр
        /// </summary>
        public static readonly Guid Kilometer = new("00000000-0000-0000-0000-000000000011");

        #endregion

        #region Площадь

        /// <summary>
        /// Квадратный сантиметр
        /// </summary>
        public static readonly Guid SquareCentimeter = new("00000000-0000-0000-0000-000000000012");

        /// <summary>
        /// Квадратный метр
        /// </summary>
        public static readonly Guid SquareMeter = new("00000000-0000-0000-0000-000000000013");

        /// <summary>
        /// Гектар
        /// </summary>
        public static readonly Guid Hectare = new("00000000-0000-0000-0000-000000000014");

        #endregion

        #region Объём

        /// <summary>
        /// Миллилитр
        /// </summary>
        public static readonly Guid Milliliter = new("00000000-0000-0000-0000-000000000015");

        /// <summary>
        /// Литр
        /// </summary>
        public static readonly Guid Liter = new("00000000-0000-0000-0000-000000000016");

        /// <summary>
        /// Кубический метр
        /// </summary>
        public static readonly Guid CubicMeter = new("00000000-0000-0000-0000-000000000017");

        #endregion

        #region Время

        /// <summary>
        /// Минута
        /// </summary>
        public static readonly Guid Minute = new("00000000-0000-0000-0000-000000000018");

        /// <summary>
        /// Час
        /// </summary>
        public static readonly Guid Hour = new("00000000-0000-0000-0000-000000000019");

        /// <summary>
        /// День
        /// </summary>
        public static readonly Guid Day = new("00000000-0000-0000-0000-000000000020");

        #endregion

        #region Проценты и доли

        /// <summary>
        /// Доля
        /// </summary>
        public static readonly Guid Fraction = new("00000000-0000-0000-0000-000000000021");

        /// <summary>
        /// Процент
        /// </summary>
        public static readonly Guid Percent = new("00000000-0000-0000-0000-000000000022");

        #endregion

        #region Энергия / мощность

        /// <summary>
        /// Ватт
        /// </summary>
        public static readonly Guid Watt = new("00000000-0000-0000-0000-000000000023");

        /// <summary>
        /// Киловатт
        /// </summary>
        public static readonly Guid Kilowatt = new("00000000-0000-0000-0000-000000000024");

        /// <summary>
        /// Киловатт-час
        /// </summary>
        public static readonly Guid KilowattHour = new("00000000-0000-0000-0000-000000000025");

        #endregion
    }
}
