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
                var userId = Guid.NewGuid();
                var now = DateTime.Now;

                Insert.IntoTable("Users")
                    .InSchema("auth")
                    .Row(new
                    {
                        Id = userId,
                        Name = "test_user",
                        DisplayName = "Тестовый пользователь",
                        Email = "test_user@test.ru",
                        IsEmailVerified = true,
                        PhoneNumber = "9999999999",
                        PhoneCountryCode = "7",
                        IsPhoneNumberVerified = true,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    })
                ;

                Insert.IntoTable("UserPasswords")
                    .InSchema("auth")
                    .Row(new
                    {
                        UserId = userId,
                        PasswordHash = "aS/CTK5Pn/G6A/VloU+/XZN/8wOYBceS05qhDMGz26e5v3Jhv9zDzcpzd5wf7hl60x7vK7T5tDKQf7M0txKO5w==",
                        PasswordSalt = "BkAWnkzut8FgiCMfjbzxuo9YoYgluv/oR6/12JABWAxunaQXGccP0yt5o954OPk9VX2U5y9aHUd5zlO0tjoFfmSrYqgR5akEyKnA0FqnXRac3RJ+frBDCCnGSD3HrbaeVfzxt3I05L7YYhym23nZeMcZiUSDI5CX9m/5gLhwRB4=",
                        IsActive = true,
                        CreatedAt = now
                    })
                ;

                Insert.IntoTable("UserPasswordHistory")
                    .InSchema("auth")
                    .Row(new
                    {
                        UserId = userId,
                        PasswordHash = "aS/CTK5Pn/G6A/VloU+/XZN/8wOYBceS05qhDMGz26e5v3Jhv9zDzcpzd5wf7hl60x7vK7T5tDKQf7M0txKO5w==",
                        UsedFromAt = now,
                        CreatedAt = now
                    })
                ;
            }
        }

        public override void Down()
        {
            //empty, not using
        }
    }
}
