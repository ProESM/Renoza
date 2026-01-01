using FluentMigrator;
using Renoza.DbMigration.Attributes;
using Renoza.DbMigration.Enums;

namespace Renoza.DbMigration.Profile
{
    [Profile("Development")]
    public class DevelopmentProfile : FluentMigrator.Migration
    {
        public override void Up()
        {
            //if (Schema.Schema("public").Table("Countries").Exists())
            //{
            //    var now = DateTime.UtcNow;
            //    var countries = typeof(Country).GetMembers()
            //        .Select(x => x.GetCustomAttributes(typeof(CountryDetailsAttribute), false))
            //        .SelectMany(x => x.Cast<CountryDetailsAttribute>());

            //    foreach (var country in countries)
            //    {
            //        Insert.IntoTable("Countries")
            //            .InSchema("public")
            //            .Row(new
            //            {
            //                Name = country.Name,
            //                Code = country.Code,
            //                IsActive = country.IsActive,
            //                CreatedAt = now,
            //                UpdatedAt = now
            //            });

            //        Execute.Sql($@"INSERT INTO public.""PhoneCountryCodes""
            //            (""Code"", ""PhoneFormat"", ""CountryId"")
            //            SELECT '{country.PhoneCountryCode}', '{country.PhoneFormat}', ""Id"" AS ""CountryId""
            //            FROM public.""Countries""
            //            WHERE ""Code"" = '{country.Code}';
            //        ");
            //    }
            //}

            //if (Schema.Schema("auth").Table("Users").Exists())
            //{
            //    var userId = Guid.NewGuid();
            //    var roleMember = Role.Administrator.GetType()
            //        .GetMember(Role.Administrator.ToString())
            //        .First();
            //    var roleId = ((RoleDetailsAttribute)roleMember
            //        .GetCustomAttributes(typeof(RoleDetailsAttribute), false)
            //        .First())
            //        .Id;
            //    var now = DateTime.UtcNow;

            //    Insert.IntoTable("Users")
            //        .InSchema("auth")
            //        .Row(new
            //        {
            //            Id = userId,
            //            Name = "test_user",
            //            DisplayName = "Тестовый пользователь",
            //            Email = "test_user@test.ru",
            //            IsEmailVerified = true,
            //            PhoneNumber = "9999999999",
            //            PhoneCountryCode = "7",
            //            IsPhoneNumberVerified = true,
            //            IsActive = true,
            //            CreatedAt = now,
            //            UpdatedAt = now
            //        })
            //    ;

            //    // тут внесён пароль Qwerty123!
            //    Insert.IntoTable("UserPasswords")
            //        .InSchema("auth")
            //        .Row(new
            //        {
            //            UserId = userId,
            //            PasswordHash = "FkAvVqbwhy0v0vz+5sLrl6ia8p/RESG8yqPjGcm5jHW6OAA7J7nCbMJG5u7H1+ketml6fDPFzjz2Hu720m7Hbw==",
            //            PasswordSalt = "ix3Zt5I+TcJZwR/3sSKubG/RlgGmj8l2gZ+ZcDgIvapzcLh6KeqYEkT7OyHUFcAioz/M6mvCXtzDTXaYkqmTIqeSJhJp5txOIXNcdm7i0mDPjqErwj3FRXCB2vLGHckb/BmXXv3tYbPs2SeMWU9eWKvLUsKJrcHzWcplXnStids=",
            //            IsActive = true,
            //            CreatedAt = now
            //        })
            //    ;

            //    Insert.IntoTable("UserPasswordHistory")
            //        .InSchema("auth")
            //        .Row(new
            //        {
            //            UserId = userId,
            //            PasswordHash = "FkAvVqbwhy0v0vz+5sLrl6ia8p/RESG8yqPjGcm5jHW6OAA7J7nCbMJG5u7H1+ketml6fDPFzjz2Hu720m7Hbw==",
            //            UsedFromAt = now,
            //            CreatedAt = now
            //        })
            //    ;

            //    // Назначаем роль администратора
            //    Insert.IntoTable("UserRoles")
            //    .InSchema("auth")
            //    .Row(new
            //    {
            //        UserId = userId,
            //        RoleId = roleId,
            //        IsActive = true,
            //        CreatedAt = now,
            //        UpdatedAt = now
            //    });

            //    //// Получаем ID роли Customer
            //    //var customerRoleMember = Role.Customer.GetType()
            //    //    .GetMember(Role.Customer.ToString())
            //    //    .First();
            //    //var customerRoleId = ((RoleDetailsAttribute)customerRoleMember
            //    //    .GetCustomAttributes(typeof(RoleDetailsAttribute), false)
            //    //    .First())
            //    //    .Id;

            //    //// Назначаем роль заказчика
            //    //Insert.IntoTable("UserRoles")
            //    //    .InSchema("auth")
            //    //    .Row(new
            //    //    {
            //    //        UserId = userId,
            //    //        RoleId = customerRoleId,
            //    //        IsActive = true,
            //    //        CreatedAt = now,
            //    //        UpdatedAt = now
            //    //    });

            //    //// Получаем ID роли Worker
            //    //var workerRoleMember = Role.Worker.GetType()
            //    //    .GetMember(Role.Worker.ToString())
            //    //    .First();
            //    //var workerRoleId = ((RoleDetailsAttribute)workerRoleMember
            //    //    .GetCustomAttributes(typeof(RoleDetailsAttribute), false)
            //    //    .First())
            //    //    .Id;

            //    //// Назначаем роль работника
            //    //Insert.IntoTable("UserRoles")
            //    //    .InSchema("auth")
            //    //    .Row(new
            //    //    {
            //    //        UserId = userId,
            //    //        RoleId = workerRoleId,
            //    //        IsActive = true,
            //    //        CreatedAt = now,
            //    //        UpdatedAt = now
            //    //    });

            //    //// Создаем профиль заказчика
            //    //Insert.IntoTable("CustomerProfiles")
            //    //    .InSchema("public")
            //    //    .Row(new
            //    //    {
            //    //        Id = Guid.NewGuid(),
            //    //        UserId = userId,
            //    //        CompanyName = "ООО Тестовая компания",
            //    //        TaxId = "1234567890",
            //    //        BillingAddress = "г. Москва, ул. Тестовая, д. 1",
            //    //        CreditLimit = 100000.00m,
            //    //        IsActive = true,
            //    //        CreatedAt = now,
            //    //        UpdatedAt = now
            //    //    });

            //    //// Создаем профиль работника
            //    //var certificationsArray = new[] { "Сертификат электрика", "Сертификат сантехника" };
            //    //var certificationsAsString = "{\"" + string.Join("\",\"", certificationsArray) + "\"}";
            //    //Insert.IntoTable("WorkerProfiles")
            //    //    .InSchema("public")
            //    //    .Row(new
            //    //    {
            //    //        Id = Guid.NewGuid(),
            //    //        UserId = userId,
            //    //        Specialization = "Универсальный специалист",
            //    //        TeamSize = 3,
            //    //        Certifications = certificationsAsString,
            //    //        ProfessionalStartDate = new DateTime(2015, 1, 1),
            //    //        IsAvailable = true,
            //    //        Rating = 4.8m,
            //    //        IsActive = true,
            //    //        CreatedAt = now,
            //    //        UpdatedAt = now
            //    //    });
            //}
        }

        public override void Down()
        {
            //empty, not using
        }
    }
}
