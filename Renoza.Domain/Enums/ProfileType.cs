namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Тип профиля (физическое или юридическое лицо)
    /// </summary>
    public enum ProfileType : short
    {
        /// <summary>
        /// Физическое лицо
        /// </summary>
        Person = 1,

        /// <summary>
        /// Юридическое лицо (компания)
        /// </summary>
        Company = 2
    }
}
