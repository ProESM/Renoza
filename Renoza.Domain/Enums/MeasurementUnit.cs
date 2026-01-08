namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Единица измерения
    /// </summary>
    public enum MeasurementUnit
    {
        #region Количество

        /// <summary>
        /// Штука
        /// </summary>
        Piece,

        /// <summary>
        /// Комплект
        /// </summary>
        Set,

        /// <summary>
        /// Упаковка
        /// </summary>
        Package,

        /// <summary>
        /// Пара
        /// </summary>
        Pair,

        #endregion

        #region Масса

        /// <summary>
        /// Грамм
        /// </summary>
        Gram,

        /// <summary>
        /// Килограмм
        /// </summary>
        Kilogram,

        /// <summary>
        /// Тонна
        /// </summary>
        Tonne,

        #endregion

        #region Длина

        /// <summary>
        /// Миллиметр
        /// </summary>
        Millimeter,

        /// <summary>
        /// Сантиметр
        /// </summary>
        Centimeter,

        /// <summary>
        /// Метр
        /// </summary>
        Meter,

        /// <summary>
        /// Километр
        /// </summary>
        Kilometer,

        #endregion

        #region Площадь

        /// <summary>
        /// Квадратный сантиметр
        /// </summary>
        SquareCentimeter,

        /// <summary>
        /// Квадратный метр
        /// </summary>
        SquareMeter,

        /// <summary>
        /// Гектар
        /// </summary>
        Hectare,

        #endregion

        #region Объём

        /// <summary>
        /// Миллилитр
        /// </summary>
        Milliliter,

        /// <summary>
        /// Литр
        /// </summary>
        Liter,

        /// <summary>
        /// Кубический метр
        /// </summary>
        CubicMeter,

        #endregion

        #region Время

        /// <summary>
        /// Минута
        /// </summary>
        Minute,

        /// <summary>
        /// Час
        /// </summary>
        Hour,

        /// <summary>
        /// День
        /// </summary>
        Day,

        #endregion

        #region Проценты и доли

        /// <summary>
        /// Доля
        /// </summary>
        Fraction,

        /// <summary>
        /// Процент
        /// </summary>
        Percent,

        #endregion

        #region Энергия / мощность

        /// <summary>
        /// Ватт
        /// </summary>
        Watt,

        /// <summary>
        /// Киловатт
        /// </summary>
        Kilowatt,

        /// <summary>
        /// Киловатт-час
        /// </summary>
        KilowattHour

        #endregion
    }
}
