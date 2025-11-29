using System.Data;
using FluentMigrator;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025112201, "Добавление таблиц верификации email и телефона")]
    public class AddVerificationTables : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Схема auth

            // Таблица верификации email
            if (!Schema.Schema("auth").Table("EmailVerifications").Exists())
            {
                Create.Table("EmailVerifications")
                    .InSchema("auth")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("FK_EmailVerifications_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("Email").AsString(256).NotNullable()
                    .WithColumn("VerificationCode").AsString(10).NotNullable()
                    .WithColumn("IsVerified").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("ExpiresAt").AsCustom("timestamp with time zone").NotNullable()
                    .WithColumn("VerifiedAt").AsCustom("timestamp with time zone").Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_EmailVerifications_UserId")
                    .OnTable("EmailVerifications")
                    .InSchema("auth")
                    .OnColumn("UserId");

                Create.Index("IDX_EmailVerifications_Email")
                    .OnTable("EmailVerifications")
                    .InSchema("auth")
                    .OnColumn("Email");

                Create.Index("IDX_EmailVerifications_VerificationCode")
                    .OnTable("EmailVerifications")
                    .InSchema("auth")
                    .OnColumn("VerificationCode");
            }

            // Таблица верификации телефона
            if (!Schema.Schema("auth").Table("PhoneVerifications").Exists())
            {
                Create.Table("PhoneVerifications")
                    .InSchema("auth")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("FK_PhoneVerifications_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("PhoneNumber").AsString(50).NotNullable()
                    .WithColumn("PhoneCountryCode").AsString(6).NotNullable()
                    .WithColumn("VerificationCode").AsString(10).NotNullable()
                    .WithColumn("IsVerified").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("ExpiresAt").AsCustom("timestamp with time zone").NotNullable()
                    .WithColumn("VerifiedAt").AsCustom("timestamp with time zone").Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_PhoneVerifications_UserId")
                    .OnTable("PhoneVerifications")
                    .InSchema("auth")
                    .OnColumn("UserId");

                // Композитный индекс по PhoneCountryCode и PhoneNumber
                Create.Index("IDX_PhoneVerifications_PhoneCountryCode_PhoneNumber")
                    .OnTable("PhoneVerifications")
                    .InSchema("auth")
                    .OnColumn("PhoneCountryCode").Ascending()
                    .OnColumn("PhoneNumber").Ascending();

                Create.Index("IDX_PhoneVerifications_VerificationCode")
                    .OnTable("PhoneVerifications")
                    .InSchema("auth")
                    .OnColumn("VerificationCode");
            }

            #endregion
        }

        public override void Down()
        {
            #region Схема auth

            // Удаляем таблицу верификации телефона
            if (Schema.Schema("auth").Table("PhoneVerifications").Exists())
            {
                if (Schema.Schema("auth").Table("PhoneVerifications").Index("IDX_PhoneVerifications_UserId").Exists())
                {
                    Delete.Index("IDX_PhoneVerifications_UserId").OnTable("PhoneVerifications").InSchema("auth");
                }
                if (Schema.Schema("auth").Table("PhoneVerifications").Index("IDX_PhoneVerifications_PhoneCountryCode_PhoneNumber").Exists())
                {
                    Delete.Index("IDX_PhoneVerifications_PhoneCountryCode_PhoneNumber").OnTable("PhoneVerifications").InSchema("auth");
                }
                if (Schema.Schema("auth").Table("PhoneVerifications").Index("IDX_PhoneVerifications_VerificationCode").Exists())
                {
                    Delete.Index("IDX_PhoneVerifications_VerificationCode").OnTable("PhoneVerifications").InSchema("auth");
                }
                Delete.Table("PhoneVerifications").InSchema("auth");
            }

            // Удаляем таблицу верификации email
            if (Schema.Schema("auth").Table("EmailVerifications").Exists())
            {
                if (Schema.Schema("auth").Table("EmailVerifications").Index("IDX_EmailVerifications_UserId").Exists())
                {
                    Delete.Index("IDX_EmailVerifications_UserId").OnTable("EmailVerifications").InSchema("auth");
                }
                if (Schema.Schema("auth").Table("EmailVerifications").Index("IDX_EmailVerifications_Email").Exists())
                {
                    Delete.Index("IDX_EmailVerifications_Email").OnTable("EmailVerifications").InSchema("auth");
                }
                if (Schema.Schema("auth").Table("EmailVerifications").Index("IDX_EmailVerifications_VerificationCode").Exists())
                {
                    Delete.Index("IDX_EmailVerifications_VerificationCode").OnTable("EmailVerifications").InSchema("auth");
                }
                Delete.Table("EmailVerifications").InSchema("auth");
            }

            #endregion
        }
    }
}
