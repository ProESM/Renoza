using System.Data;
using FluentMigrator;
using Renoza.DbMigration.Attributes;
using Renoza.DbMigration.Enums;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025110902, "Добавление ролей и профилей пользователей")]
    public class AddRolesAndProfiles : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Схема auth

            // Таблица ролей
            if (!Schema.Schema("auth").Table("Roles").Exists())
            {
                Create.Table("Roles")
                    .InSchema("auth")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("Name").AsString(100).NotNullable().Unique()
                    .WithColumn("Description").AsString(500).Nullable()
                    .WithColumn("IsSystemRole").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                var now = DateTime.UtcNow;
                var roles = typeof(Role).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(RoleDetailsAttribute), false))
                    .SelectMany(x => x.Cast<RoleDetailsAttribute>());

                foreach (var role in roles)
                {
                    Insert.IntoTable("Roles")
                        .InSchema("auth")
                        .Row(new
                        {
                            Id = role.Id,
                            Name = role.Name,
                            Description = role.Description,
                            IsSystemRole = role.IsSystemRole,
                            IsActive = role.IsActive,
                            CreatedAt = now,
                            UpdatedAt = now
                        });
                }
            }

            // Таблица связей пользователей и ролей (многие-ко-многим)
            if (!Schema.Schema("auth").Table("UserRoles").Exists())
            {
                Create.Table("UserRoles")
                    .InSchema("auth")
                    .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("FK_UserRoles_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("RoleId").AsGuid().NotNullable().ForeignKey("FK_UserRoles_RoleId", "auth", "Roles", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // Композитный первичный ключ
                Create.PrimaryKey("PK_UserRoles")
                    .OnTable("UserRoles")
                    .WithSchema("auth")
                    .Columns("UserId", "RoleId");

                // Индексы для оптимизации запросов
                Create.Index("IDX_UserRoles_UserId")
                    .OnTable("UserRoles")
                    .InSchema("auth")
                    .OnColumn("UserId");

                Create.Index("IDX_UserRoles_RoleId")
                    .OnTable("UserRoles")
                    .InSchema("auth")
                    .OnColumn("RoleId");
            }

            #endregion

            #region Схема public

            // Таблица профилей заказчиков
            if (!Schema.Schema("public").Table("CustomerProfiles").Exists())
            {
                Create.Table("CustomerProfiles")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("UserId").AsGuid().NotNullable().Unique().ForeignKey("FK_CustomerProfiles_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("CompanyName").AsString(256).Nullable()
                    .WithColumn("TaxId").AsString(50).Nullable()
                    .WithColumn("BillingAddress").AsString(500).Nullable()
                    .WithColumn("CreditLimit").AsDecimal(18, 2).Nullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_CustomerProfiles_UserId")
                    .OnTable("CustomerProfiles")
                    .InSchema("public")
                    .OnColumn("UserId");

                Create.Index("IDX_CustomerProfiles_IsActive")
                    .OnTable("CustomerProfiles")
                    .InSchema("public")
                    .OnColumn("IsActive");
            }

            // Таблица профилей работников
            if (!Schema.Schema("public").Table("WorkerProfiles").Exists())
            {
                Create.Table("WorkerProfiles")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("UserId").AsGuid().NotNullable().Unique().ForeignKey("FK_WorkerProfiles_UserId", "auth", "Users", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("Specialization").AsString(256).Nullable()
                    .WithColumn("TeamSize").AsInt32().Nullable()
                    .WithColumn("Certifications").AsCustom("text[]").Nullable()
                    .WithColumn("ProfessionalStartDate").AsDate().Nullable()
                    .WithColumn("IsAvailable").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("Rating").AsDecimal(3, 2).Nullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_WorkerProfiles_UserId")
                    .OnTable("WorkerProfiles")
                    .InSchema("public")
                    .OnColumn("UserId");

                Create.Index("IDX_WorkerProfiles_IsAvailable")
                    .OnTable("WorkerProfiles")
                    .InSchema("public")
                    .OnColumn("IsAvailable");

                Create.Index("IDX_WorkerProfiles_IsActive")
                    .OnTable("WorkerProfiles")
                    .InSchema("public")
                    .OnColumn("IsActive");
            }

            #endregion
        }

        public override void Down()
        {
            #region Схема public

            // Удаляем профили работников
            if (Schema.Schema("public").Table("WorkerProfiles").Exists())
            {
                if (Schema.Schema("public").Table("WorkerProfiles").Index("IDX_WorkerProfiles_UserId").Exists())
                {
                    Delete.Index("IDX_WorkerProfiles_UserId").OnTable("WorkerProfiles").InSchema("public");
                }
                if (Schema.Schema("public").Table("WorkerProfiles").Index("IDX_WorkerProfiles_IsAvailable").Exists())
                {
                    Delete.Index("IDX_WorkerProfiles_IsAvailable").OnTable("WorkerProfiles").InSchema("public");
                }
                if (Schema.Schema("public").Table("WorkerProfiles").Index("IDX_WorkerProfiles_IsActive").Exists())
                {
                    Delete.Index("IDX_WorkerProfiles_IsActive").OnTable("WorkerProfiles").InSchema("public");
                }
                Delete.Table("WorkerProfiles").InSchema("public");
            }

            // Удаляем профили заказчиков
            if (Schema.Schema("public").Table("CustomerProfiles").Exists())
            {
                if (Schema.Schema("public").Table("CustomerProfiles").Index("IDX_CustomerProfiles_UserId").Exists())
                {
                    Delete.Index("IDX_CustomerProfiles_UserId").OnTable("CustomerProfiles").InSchema("public");
                }
                if (Schema.Schema("public").Table("CustomerProfiles").Index("IDX_CustomerProfiles_IsActive").Exists())
                {
                    Delete.Index("IDX_CustomerProfiles_IsActive").OnTable("CustomerProfiles").InSchema("public");
                }
                Delete.Table("CustomerProfiles").InSchema("public");
            }

            #endregion

            #region Схема auth

            // Удаляем связи пользователей и ролей
            if (Schema.Schema("auth").Table("UserRoles").Exists())
            {
                if (Schema.Schema("auth").Table("UserRoles").Index("IDX_UserRoles_UserId").Exists())
                {
                    Delete.Index("IDX_UserRoles_UserId").OnTable("UserRoles").InSchema("auth");
                }
                if (Schema.Schema("auth").Table("UserRoles").Index("IDX_UserRoles_RoleId").Exists())
                {
                    Delete.Index("IDX_UserRoles_RoleId").OnTable("UserRoles").InSchema("auth");
                }
                Delete.Table("UserRoles").InSchema("auth");
            }

            // Удаляем роли
            if (Schema.Schema("auth").Table("Roles").Exists())
            {
                Delete.Table("Roles").InSchema("auth");
            }

            #endregion
        }
    }
}
