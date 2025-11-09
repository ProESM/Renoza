namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройка политики паролей
    /// </summary>
    public class PasswordPolicyOptions
    {
        public int MinLength { get; set; } = 8;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireDigit { get; set; } = true;
        public bool RequireSpecialCharacter { get; set; } = true;
        public int HistorySize { get; set; } = 5;
        public int ExpiryDays { get; set; } = 90;
        public int MaxFailedAttempts { get; set; } = 5;
        public int LockoutMinutes { get; set; } = 30;
    }
}
