using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Renoza.Domain.Options;

namespace Renoza.Backend
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Настройка JWT Authentication
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }

        public static IServiceCollection AddSwaggerGenerator(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Добавляем определение схемы безопасности Bearer
                var securityScheme = new OpenApiSecurityScheme();
                securityScheme.Description = "Введите JWT токен в формате: Bearer {ваш токен}";
                securityScheme.Name = "Authorization";
                securityScheme.Scheme = "bearer";
                securityScheme.BearerFormat = "JWT";

                options.AddSecurityDefinition("Bearer", securityScheme);

                // Добавляем требование безопасности для всех операций
                options.AddSecurityRequirement(document =>
                {
                    var requirement = new OpenApiSecurityRequirement();
                    var scheme = new OpenApiSecuritySchemeReference("Bearer", document);
                    requirement.Add(scheme, new List<string>());
                    return requirement;
                });
            });

            return services;
        }
    }
}
