using FluentMigrator;

namespace Renoza.DbMigration.Profile
{
    [Profile("Development")]
    public class DevelopmentProfile : FluentMigrator.Migration
    {
        public override void Up()
        {
            if (Schema.Schema("auth").Table("Users").Exists())
            {
                //var userId = Guid.NewGuid();
                //var now = DateTime.UtcNow;

                //Insert.IntoTable("Users")
                //    .InSchema("auth")
                //    .Row(new
                //    {
                //        Id = userId,
                //        Name = "test_user",
                //        DisplayName = "Тестовый пользователь",
                //        Email = "test_user@test.ru",
                //        IsEmailVerified = true,
                //        PhoneNumber = "9999999999",
                //        PhoneCountryCode = "7",
                //        IsPhoneNumberVerified = true,
                //        IsActive = true,
                //        CreatedAt = now,
                //        UpdatedAt = now
                //    })
                //;

                //// тут внесён пароль Qwerty123!
                //Insert.IntoTable("UserPasswords")
                //    .InSchema("auth")
                //    .Row(new
                //    {
                //        UserId = userId,
                //        PasswordHash = "FkAvVqbwhy0v0vz+5sLrl6ia8p/RESG8yqPjGcm5jHW6OAA7J7nCbMJG5u7H1+ketml6fDPFzjz2Hu720m7Hbw==",
                //        PasswordSalt = "ix3Zt5I+TcJZwR/3sSKubG/RlgGmj8l2gZ+ZcDgIvapzcLh6KeqYEkT7OyHUFcAioz/M6mvCXtzDTXaYkqmTIqeSJhJp5txOIXNcdm7i0mDPjqErwj3FRXCB2vLGHckb/BmXXv3tYbPs2SeMWU9eWKvLUsKJrcHzWcplXnStids=",
                //        IsActive = true,
                //        CreatedAt = now
                //    })
                //;

                //Insert.IntoTable("UserPasswordHistory")
                //    .InSchema("auth")
                //    .Row(new
                //    {
                //        UserId = userId,
                //        PasswordHash = "FkAvVqbwhy0v0vz+5sLrl6ia8p/RESG8yqPjGcm5jHW6OAA7J7nCbMJG5u7H1+ketml6fDPFzjz2Hu720m7Hbw==",
                //        UsedFromAt = now,
                //        CreatedAt = now
                //    })
                //;
            }
        }

        public override void Down()
        {
            //empty, not using
        }
    }
}
