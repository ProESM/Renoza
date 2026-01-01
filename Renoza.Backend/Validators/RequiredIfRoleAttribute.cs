using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Validators
{
    /// <summary>
    /// Атрибут валидации, делающий поле обязательным для определенных ролей
    /// </summary>
    public class RequiredIfRoleAttribute : ValidationAttribute
    {
        private readonly string[] _requiredRoles;

        /// <summary>
        /// Конструктор атрибута
        /// </summary>
        /// <param name="requiredRoles">Роли, для которых поле обязательно</param>
        public RequiredIfRoleAttribute(params string[] requiredRoles)
        {
            _requiredRoles = requiredRoles;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Получаем свойство UserRole из объекта валидации
            var userRoleProperty = validationContext.ObjectType.GetProperty("UserRole");
            if (userRoleProperty == null)
            {
                return new ValidationResult("Свойство UserRole не найдено");
            }

            var userRole = userRoleProperty.GetValue(validationContext.ObjectInstance) as string;

            // Проверяем, требуется ли поле для данной роли
            if (!string.IsNullOrWhiteSpace(userRole) && _requiredRoles.Contains(userRole))
            {
                // Поле обязательно для этой роли
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(ErrorMessage ?? $"Поле обязательно для роли {userRole}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
