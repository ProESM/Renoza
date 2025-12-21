using Renoza.Backend.Helpers;
using Renoza.Domain.DI;
using Renoza.Backend.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
builder.Host.ConfigureSerilog(builder.Configuration);

Console.WriteLine(builder.Environment.EnvironmentName);

// Add services to the container.

using (var serviceProvider = MigrationHelper.CreateServices(builder.Configuration, builder.Environment.IsDevelopment()))
using (var scope = serviceProvider.CreateScope())
{
    // Put the database update into a scope to ensure
    // that all resources will be disposed.
    MigrationHelper.UpdateDatabase(scope.ServiceProvider);
}

// Настройка CORS для Frontend приложения
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Регистрируем зависимости Domain слоя (DbContext, UnitOfWork, репозитории, AutoMapper, сервисы, конфигурационные опции)
builder.Services.AddDomainCore(builder.Configuration)
    .AddJwtServices(builder.Configuration)
    .AddEmailServices(builder.Configuration)
    .AddRabbitMqServices(builder.Configuration)
    .AddRedisServices(builder.Configuration)
    .AddRateLimitingServices(builder.Configuration)
    .AddS3Services(builder.Configuration)
    .AddDocumentServices()
    ;
// Настраиваем Jwt Bearer аутентификацию
builder.Services.AddJwtBearerAuthentication(builder.Configuration);
// Настраиваем MassTransit и подключение к RabbitMQ
builder.Services.AddRabbitMqMassTransit();

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Настраиваем Swagger
builder.Services.AddSwaggerGenerator();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Включаем CORS (должен быть до Authentication и Authorization)
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Запуск приложения Renoza.Backend");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение неожиданно завершилось");
}
finally
{
    Log.CloseAndFlush();
}
