using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Тип единицы измерения
    /// </summary>
    public enum MeasurementUnitType
    {
        /// <summary>
        /// Количество (штучные)
        /// </summary>
        [MeasurementUnitTypeDetails("00000000-0000-0000-0000-000000000001", "quantity", "Количество (штучные)", "Количество (штучные)", true)]
        Quantity,
        /// <summary>
        /// Масса
        /// </summary>
        [MeasurementUnitTypeDetails("00000000-0000-0000-0000-000000000002", "weight", "Масса", "Вес товаров", true)]
        Weight,
        /// <summary>
        /// Длина
        /// </summary>
        [MeasurementUnitTypeDetails("00000000-0000-0000-0000-000000000003", "length", "Длина", "Линейные размеры", true)]
        Length,
        /// <summary>
        /// Площадь
        /// </summary>
        [MeasurementUnitTypeDetails("00000000-0000-0000-0000-000000000004", "area", "Площадь", "Покрытия, отделка, недвижимость", true)]
        Area,
        /// <summary>
        /// Объём
        /// </summary>
        [MeasurementUnitTypeDetails("00000000-0000-0000-0000-000000000005", "volume", "Объём", "Жидкости, сыпучие материалы", true)]
        Volume,
        /// <summary>
        /// Время
        /// </summary>
        [MeasurementUnitTypeDetails("00000000-0000-0000-0000-000000000006", "time", "Время", "Услуги, аренда, подписки", true)]
        Time,
        /// <summary>
        /// Проценты и доли
        /// </summary>
        [MeasurementUnitTypeDetails("00000000-0000-0000-0000-000000000007", "ratio", "Проценты и доли", "Скидки, комиссии, НДС", true)]
        Ratio,
        /// <summary>
        /// Энергия / мощность
        /// </summary>
        [MeasurementUnitTypeDetails("00000000-0000-0000-0000-000000000008", "XXXXXXXXXXX", "Энергия / мощность", "Коммуналка, техника", true)]
        Energy
    }
}
