using FluentMigrator;
using System.Data;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025110905, "Добавление профилей пользователей (TPT) и системы участников компаний")]
    public class AddProfiles : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Базовая таблица Profiles (TPT)

            // Создаем базовую таблицу Profiles для паттерна Table Per Type
            Create.Table("Profiles")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable().Unique()
                    .ForeignKey("FK_Profiles_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IDX_Profiles_UserId")
                .OnTable("Profiles")
                .InSchema("public")
                .OnColumn("UserId");

            Create.Index("IDX_Profiles_IsActive")
                .OnTable("Profiles")
                .InSchema("public")
                .OnColumn("IsActive");

            #endregion

            #region Таблица профилей заказчиков (наследует Profiles)

            Create.Table("CustomerProfiles")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                    .ForeignKey("FK_CustomerProfiles_ProfileId", "public", "Profiles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("CompanyName").AsString(256).Nullable()
                .WithColumn("TaxId").AsString(50).Nullable()
                .WithColumn("BillingAddress").AsString(500).Nullable()
                .WithColumn("CreditLimit").AsDecimal(18, 2).Nullable();

            #endregion

            #region Таблица профилей работников (наследует Profiles)

            Create.Table("WorkerProfiles")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                    .ForeignKey("FK_WorkerProfiles_ProfileId", "public", "Profiles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Specialization").AsString(256).Nullable()
                .WithColumn("TeamSize").AsInt32().Nullable()
                .WithColumn("Certifications").AsCustom("text[]").Nullable()
                .WithColumn("ProfessionalStartDate").AsDate().Nullable()
                .WithColumn("IsAvailable").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("Rating").AsDecimal(3, 2).Nullable();

            Create.Index("IDX_WorkerProfiles_IsAvailable")
                .OnTable("WorkerProfiles")
                .InSchema("public")
                .OnColumn("IsAvailable");

            #endregion

            #region Таблица профилей технического надзора (наследует Profiles)

            Create.Table("TechnicalSupervisorProfiles")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                    .ForeignKey("FK_TechnicalSupervisorProfiles_ProfileId", "public", "Profiles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Specialization").AsString(256).Nullable()
                .WithColumn("Certifications").AsCustom("text[]").Nullable()
                .WithColumn("ProfessionalStartDate").AsDate().Nullable()
                .WithColumn("IsAvailable").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("Rating").AsDecimal(3, 2).Nullable();

            Create.Index("IDX_TechnicalSupervisorProfiles_IsAvailable")
                .OnTable("TechnicalSupervisorProfiles")
                .InSchema("public")
                .OnColumn("IsAvailable");

            #endregion

            #region Таблица участников компаний

            Create.Table("CompanyMembers")
                .InSchema("public")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable()
                    .ForeignKey("FK_CompanyMembers_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                .WithColumn("CompanyProfileId").AsGuid().NotNullable()
                    .ForeignKey("FK_CompanyMembers_CompanyProfileId", "public", "CompanyProfiles", "Id").OnDelete(Rule.Cascade)
                .WithColumn("MemberRoleId").AsGuid().NotNullable()
                    .ForeignKey("FK_CompanyMembers_MemberRoleId", "company", "MemberRoles", "Id").OnDelete(Rule.None)
                .WithColumn("Position").AsString(256).Nullable()
                .WithColumn("JoinedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("LeftAt").AsCustom("timestamp with time zone").Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IDX_CompanyMembers_UserId")
                .OnTable("CompanyMembers")
                .InSchema("public")
                .OnColumn("UserId");

            Create.Index("IDX_CompanyMembers_CompanyProfileId")
                .OnTable("CompanyMembers")
                .InSchema("public")
                .OnColumn("CompanyProfileId");

            Create.Index("IDX_CompanyMembers_MemberRoleId")
                .OnTable("CompanyMembers")
                .InSchema("public")
                .OnColumn("MemberRoleId");

            Create.Index("IDX_CompanyMembers_IsActive")
                .OnTable("CompanyMembers")
                .InSchema("public")
                .OnColumn("IsActive");

            // Уникальный индекс для активных участников компании
            Execute.Sql(@"
                CREATE UNIQUE INDEX ""UC_CompanyMembers_UserId_CompanyProfileId_IsActive""
                ON public.""CompanyMembers"" (""UserId"", ""CompanyProfileId"")
                WHERE ""IsActive"" = true;
            ");

            #endregion
        }

        public override void Down()
        {
            #region Удаление таблиц в обратном порядке

            // Удаляем участников компаний
            if (Schema.Schema("public").Table("CompanyMembers").Exists())
            {
                Execute.Sql(@"DROP INDEX IF EXISTS public.""UC_CompanyMembers_UserId_CompanyProfileId_IsActive"";");

                if (Schema.Schema("public").Table("CompanyMembers").Index("IDX_CompanyMembers_IsActive").Exists())
                {
                    Delete.Index("IDX_CompanyMembers_IsActive").OnTable("CompanyMembers").InSchema("public");
                }
                if (Schema.Schema("public").Table("CompanyMembers").Index("IDX_CompanyMembers_MemberRoleId").Exists())
                {
                    Delete.Index("IDX_CompanyMembers_MemberRoleId").OnTable("CompanyMembers").InSchema("public");
                }
                if (Schema.Schema("public").Table("CompanyMembers").Index("IDX_CompanyMembers_CompanyProfileId").Exists())
                {
                    Delete.Index("IDX_CompanyMembers_CompanyProfileId").OnTable("CompanyMembers").InSchema("public");
                }
                if (Schema.Schema("public").Table("CompanyMembers").Index("IDX_CompanyMembers_UserId").Exists())
                {
                    Delete.Index("IDX_CompanyMembers_UserId").OnTable("CompanyMembers").InSchema("public");
                }
                Delete.Table("CompanyMembers").InSchema("public");
            }

            // Удаляем специализированные таблицы профилей (TPT)
            if (Schema.Schema("public").Table("TechnicalSupervisorProfiles").Exists())
            {
                if (Schema.Schema("public").Table("TechnicalSupervisorProfiles").Index("IDX_TechnicalSupervisorProfiles_IsAvailable").Exists())
                {
                    Delete.Index("IDX_TechnicalSupervisorProfiles_IsAvailable").OnTable("TechnicalSupervisorProfiles").InSchema("public");
                }
                Delete.Table("TechnicalSupervisorProfiles").InSchema("public");
            }

            if (Schema.Schema("public").Table("WorkerProfiles").Exists())
            {
                if (Schema.Schema("public").Table("WorkerProfiles").Index("IDX_WorkerProfiles_IsAvailable").Exists())
                {
                    Delete.Index("IDX_WorkerProfiles_IsAvailable").OnTable("WorkerProfiles").InSchema("public");
                }
                Delete.Table("WorkerProfiles").InSchema("public");
            }

            if (Schema.Schema("public").Table("CustomerProfiles").Exists())
            {
                Delete.Table("CustomerProfiles").InSchema("public");
            }

            // Удаляем базовую таблицу Profiles
            if (Schema.Schema("public").Table("Profiles").Exists())
            {
                if (Schema.Schema("public").Table("Profiles").Index("IDX_Profiles_IsActive").Exists())
                {
                    Delete.Index("IDX_Profiles_IsActive").OnTable("Profiles").InSchema("public");
                }
                if (Schema.Schema("public").Table("Profiles").Index("IDX_Profiles_UserId").Exists())
                {
                    Delete.Index("IDX_Profiles_UserId").OnTable("Profiles").InSchema("public");
                }
                Delete.Table("Profiles").InSchema("public");
            }

            #endregion
        }
    }
}
