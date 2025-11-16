using System.Data;
using FluentMigrator;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025110901, "Инициализация базы данных")]
    public class InitDb : FluentMigrator.Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("public").Exists())
            {
                Create.Schema("public");
            }

            #region Таблицы и представления

            #region Схема auth

            #region Таблицы

            if (!Schema.Schema("auth").Exists())
            {
                Create.Schema("auth");
            }

            if (!Schema.Schema("auth").Table("Users").Exists())
            {
                Create.Table("Users")
                    .InSchema("auth")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("Name").AsString(256).NotNullable().Unique()
                    .WithColumn("DisplayName").AsString(256).NotNullable()
                    .WithColumn("Email").AsString(256).NotNullable().Unique()
                    .WithColumn("IsEmailVerified").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("PhoneNumber").AsString(50).NotNullable().Unique()
                    .WithColumn("PhoneCountryCode").AsString(6).NotNullable()
                    .WithColumn("IsPhoneNumberVerified").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);
            }

            if (!Schema.Schema("auth").Table("UserPasswords").Exists())
            {
                Create.Table("UserPasswords")
                    .InSchema("auth")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("UserId").AsGuid().Nullable().ForeignKey("FK_UserPasswords_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("PasswordHash").AsString(256).NotNullable()
                    .WithColumn("PasswordSalt").AsString(256).NotNullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("ExpiredAt").AsCustom("timestamp with time zone").Nullable();

                Create.Index("IDX_UserPasswords_UserId")
                    .OnTable("UserPasswords")
                    .InSchema("auth")
                    .OnColumn("UserId");

                Execute.Sql(@"
                    CREATE UNIQUE INDEX ""UC_UserPasswords_UserId_IsActive""
                    ON auth.""UserPasswords"" (""UserId"", ""IsActive"")
                    WHERE ""IsActive"" = true;
                ");
            }

            if (!Schema.Schema("auth").Table("UserPasswordHistory").Exists())
            {
                Create.Table("UserPasswordHistory")
                    .InSchema("auth")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("UserId").AsGuid().Nullable().ForeignKey("FK_UserPasswordHistory_UserId", "auth", "Users", "Id")
                    .OnDelete(Rule.Cascade)
                    .WithColumn("PasswordHash").AsString(256).NotNullable()
                    .WithColumn("UsedFromAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UsedToAt").AsCustom("timestamp with time zone").Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_UserPasswordHistory_UserId")
                    .OnTable("UserPasswordHistory")
                    .InSchema("auth")
                    .OnColumn("UserId");
            }

            #endregion

            #region Представления



            #endregion

            #endregion

            #region Схема public

            #region Таблицы

            if (!Schema.Schema("public").Table("Countries").Exists())
            {
                Create.Table("Countries")
                    .InSchema("public")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("Code").AsString(3).NotNullable().Unique()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);
            }
            if (!Schema.Schema("public").Table("PhoneCountryCodes").Exists())
            {
                Create.Table("PhoneCountryCodes")
                    .InSchema("public")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Code").AsString(8).NotNullable()
                    .WithColumn("PhoneFormat").AsString(50).NotNullable()
                    .WithColumn("CountryId").AsInt32().NotNullable().ForeignKey("FK_PhoneCountryCodes_CountryId", "public", "Countries", "Id").OnDelete(Rule.Cascade)
                    ;

                Create.Index("IDX_PhoneCountryCodes_CountryId")
                    .OnTable("PhoneCountryCodes")
                    .InSchema("public")
                    .OnColumn("CountryId");
            }

            #endregion

            #region Представления



            #endregion

            #endregion

            #endregion
        }

        public override void Down()
        {
            #region Схема public

            if (Schema.Schema("public").Table("PhoneCountryCodes").Exists())
            {
                Delete.Table("PhoneCountryCodes").InSchema("public");
            }
            if (Schema.Schema("public").Table("Countries").Exists())
            {
                Delete.Table("Countries").InSchema("public");
            }

            #endregion

            #region Схема auth

            if (Schema.Schema("auth").Table("UserPasswordHistory").Exists())
            {
                if (Schema.Schema("auth").Table("UserPasswordHistory").Index("IDX_UserPasswordHistory_UserId").Exists())
                {
                    Delete.Index("IDX_UserPasswordHistory_UserId").OnTable("UserPasswordHistory").InSchema("auth");
                }

                Delete.Table("UserPasswordHistory").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("UserPasswords").Exists())
            {
                if (Schema.Schema("auth").Table("UserPasswords").Index("IDX_UserPasswords_UserId").Exists())
                {
                    Delete.Index("IDX_UserPasswords_UserId").OnTable("UserPasswords").InSchema("auth");
                }
                if (Schema.Schema("auth").Table("UserPasswords").Index("UC_UserPasswords_UserId_IsActive").Exists())
                {
                    Delete.Index("UC_UserPasswords_UserId_IsActive").OnTable("UserPasswords").InSchema("auth");
                }

                Delete.Table("UserPasswords").InSchema("auth");
            }

            if (Schema.Schema("auth").Table("Users").Exists())
            {
                Delete.Table("Users").InSchema("auth");
            }

            if (Schema.Schema("auth").Exists())
            {
                Delete.Schema("auth");
            }

            #endregion
        }
    }
}
