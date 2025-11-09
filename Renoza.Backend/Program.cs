using Renoza.Backend.Helpers;
using Renoza.Domain.DI;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Environment.EnvironmentName);

// Add services to the container.

using (var serviceProvider = MigrationHelper.CreateServices(builder.Configuration, builder.Environment.IsDevelopment()))
using (var scope = serviceProvider.CreateScope())
{
    // Put the database update into a scope to ensure
    // that all resources will be disposed.
    MigrationHelper.UpdateDatabase(scope.ServiceProvider);
}

// Регистрируем зависимости Domain слоя (DbContext, UnitOfWork, репозитории, AutoMapper, сервисы, конфигурационные опции)
builder.Services.AddDomainServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
