using Renoza.Domain.Options;

namespace Renoza.Domain.Validators
{
    public class PasswordValidator
    {
        private readonly PasswordPolicyOptions _passwordPolicyOptions;

        public PasswordValidator(PasswordPolicyOptions passwordPolicyOptions)
        {
            _passwordPolicyOptions = passwordPolicyOptions;
        }

        public (bool isValid, string errorMessage) Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Пароль не может быть пустым");

            if (password.Length < _passwordPolicyOptions.MinLength)
                return (false, $"Пароль не может быть менее {_passwordPolicyOptions.MinLength} символов");

            if (_passwordPolicyOptions.RequireUppercase && !password.Any(char.IsUpper))
                return (false, "Пароль должен содержать хотя бы одну заглавную букву");

            if (_passwordPolicyOptions.RequireLowercase && !password.Any(char.IsLower))
                return (false, "Пароль должен содержать хотя бы одну строчную букву");

            if (_passwordPolicyOptions.RequireDigit && !password.Any(char.IsDigit))
                return (false, "Пароль должен содержать хотя бы одну цифру");

            if (_passwordPolicyOptions.RequireSpecialCharacter && !password.Any(ch => !char.IsLetterOrDigit(ch)))
                return (false, "Пароль должен содержать хотя бы один специальный символ");

            return (true, string.Empty);
        }
    }
}
