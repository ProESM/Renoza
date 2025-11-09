using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Configuration;
using Renoza.Domain.Options;

namespace Renoza.Domain.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetRequiredConfigurationSection<T>(this IConfiguration configuration, string sectionName)
            where T : new()
        {
            var section = configuration.GetSection(sectionName);
            if (!section.Exists())
            {
                throw new InvalidOperationException($"Required section '{sectionName}' not found.");
            }

            var settings = new T();
            section.Bind(settings);

            // Дополнительная валидация
            ValidateSettings(settings);

            return settings;
        }

        private static void ValidateSettings<T>(T settings)
        {
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(settings,
                new ValidationContext(settings), validationResults, true);

            if (!isValid)
            {
                throw new InvalidOperationException(
                    $"Settings validation failed: {string.Join(", ", validationResults)}");
            }
        }
    }
}
