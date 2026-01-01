using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Renoza.Domain.Options;
using System.Text;
using Newtonsoft.Json;

namespace Renoza.Backend.Configuration
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
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(doc =>
                {
                    var requirement = new OpenApiSecurityRequirement();
                    var reference = new OpenApiSecuritySchemeReference("Bearer", doc);
                    requirement.Add(reference, new List<string>());
                    return requirement;
                });

                // ВРЕМЕННО ОТКЛЮЧЕНО: Добавляем поддержку IFormFile и multipart/form-data
                // c.OperationFilter<FileUploadOperationFilter>();
            });

            return services;
        }

        public static IServiceCollection AddRabbitMqMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqOptions = context.GetRequiredService<RabbitMqOptions>();

                    cfg.Host($"rabbitmq://{rabbitMqOptions.Host}:{rabbitMqOptions.Port}/{rabbitMqOptions.VirtualHost}", configurator =>
                    {
                        configurator.Username(rabbitMqOptions.Username);
                        configurator.Password(rabbitMqOptions.Password);
                    });

                    // Configure Newtonsoft.Json as the serializer
                    cfg.UseNewtonsoftJsonSerializer();

                    // Optionally, configure Newtonsoft.Json settings
                    cfg.ConfigureNewtonsoftJsonSerializer(settings =>
                    {
                        settings.NullValueHandling = NullValueHandling.Include;
                        settings.DefaultValueHandling = DefaultValueHandling.Include;
                        // Add any other Newtonsoft.Json settings you require
                        return settings;
                    });
                });
            });

            return services;
        }
    }
}
