using System.Data;
using FluentMigrator;
using Renoza.DbMigration.Attributes;
using Renoza.DbMigration.Enums;

namespace Renoza.DbMigration.Migration
{
    [Migration(2025122101, "Добавление таблиц верификации компаний и профиля технического надзора")]
    public class AddCompanyVerificationTables : FluentMigrator.Migration
    {
        public override void Up()
        {
            #region Схема public

            // Таблица типов компаний
            if (!Schema.Schema("public").Table("CompanyTypes").Exists())
            {
                Create.Table("CompanyTypes")
                    .InSchema("public")
                    .WithColumn("Id").AsInt16().PrimaryKey()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // Добавляем начальные типы из Enum
                var now = DateTime.UtcNow;
                var companyTypes = typeof(CompanyType).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(CompanyTypeDetailsAttribute), false))
                    .SelectMany(x => x.Cast<CompanyTypeDetailsAttribute>());

                foreach (var companyType in companyTypes)
                {
                    Insert.IntoTable("CompanyTypes")
                        .InSchema("public")
                        .Row(new
                        {
                            Id = companyType.Id,
                            Name = companyType.Name,
                            IsActive = companyType.IsActive,
                            CreatedAt = now,
                            UpdatedAt = now
                        });
                }
            }

            // Таблица статусов заданий на верификацию компаний
            if (!Schema.Schema("public").Table("CompanyVerificationJobStatuses").Exists())
            {
                Create.Table("CompanyVerificationJobStatuses")
                    .InSchema("public")
                    .WithColumn("Id").AsInt16().PrimaryKey()
                    .WithColumn("Name").AsString(256).NotNullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                // Добавляем начальные статусы из Enum
                var now = DateTime.UtcNow;
                var statuses = typeof(CompanyVerificationJobStatus).GetMembers()
                    .Select(x => x.GetCustomAttributes(typeof(CompanyVerificationJobStatusDetailsAttribute), false))
                    .SelectMany(x => x.Cast<CompanyVerificationJobStatusDetailsAttribute>());

                foreach (var status in statuses)
                {
                    Insert.IntoTable("CompanyVerificationJobStatuses")
                        .InSchema("public")
                        .Row(new
                        {
                            Id = status.Id,
                            Name = status.Name,
                            IsActive = status.IsActive,
                            CreatedAt = now,
                            UpdatedAt = now
                        });
                }
            }

            // Таблица профилей компаний (общая для ремонтных бригад и технического надзора)
            if (!Schema.Schema("public").Table("CompanyProfiles").Exists())
            {
                Create.Table("CompanyProfiles")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("Inn").AsString(12).NotNullable()
                    .WithColumn("CompanyTypeId").AsInt16().NotNullable().WithDefaultValue(0).ForeignKey("FK_CompanyProfiles_CompanyTypeId", "public", "CompanyTypes", "Id").OnDelete(Rule.None)
                    .WithColumn("CompanyVerificationId").AsGuid().Nullable()
                    .WithColumn("IsCompanyVerified").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("CompanyVerifiedAt").AsCustom("timestamp with time zone").Nullable()
                    .WithColumn("FullName").AsString(512).Nullable()
                    .WithColumn("ShortName").AsString(256).Nullable()
                    .WithColumn("Ogrn").AsString(15).Nullable()
                    .WithColumn("Kpp").AsString(9).Nullable()
                    .WithColumn("Address").AsString(1024).Nullable()
                    .WithColumn("DirectorName").AsString(256).Nullable()
                    .WithColumn("RegistrationDate").AsCustom("date").Nullable()
                    .WithColumn("Status").AsString(50).Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_CompanyProfiles_Inn")
                    .OnTable("CompanyProfiles")
                    .InSchema("public")
                    .OnColumn("Inn");

                Create.Index("IDX_CompanyProfiles_IsCompanyVerified")
                    .OnTable("CompanyProfiles")
                    .InSchema("public")
                    .OnColumn("IsCompanyVerified");
            }

            // Таблица профилей технического надзора
            if (!Schema.Schema("public").Table("TechnicalSupervisorProfiles").Exists())
            {
                Create.Table("TechnicalSupervisorProfiles")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("UserId").AsGuid().NotNullable()
                    .WithColumn("CompanyProfileId").AsGuid().NotNullable().ForeignKey("FK_TechnicalSupervisorProfiles_CompanyProfileId", "public", "CompanyProfiles", "Id").OnDelete(Rule.None)
                    .WithColumn("Specialization").AsString(256).Nullable()
                    .WithColumn("Certifications").AsCustom("text[]").Nullable()
                    .WithColumn("ProfessionalStartDate").AsDate().Nullable()
                    .WithColumn("IsAvailable").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("Rating").AsDecimal().Nullable()
                    .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_TechnicalSupervisorProfiles_UserId")
                    .OnTable("TechnicalSupervisorProfiles")
                    .InSchema("public")
                    .OnColumn("UserId");

                Create.Index("IDX_TechnicalSupervisorProfiles_CompanyProfileId")
                    .OnTable("TechnicalSupervisorProfiles")
                    .InSchema("public")
                    .OnColumn("CompanyProfileId");

                Create.Index("IDX_TechnicalSupervisorProfiles_IsActive")
                    .OnTable("TechnicalSupervisorProfiles")
                    .InSchema("public")
                    .OnColumn("IsActive");
            }

            // Таблица результатов верификации компаний
            if (!Schema.Schema("public").Table("CompanyVerifications").Exists())
            {
                Create.Table("CompanyVerifications")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("CompanyProfileId").AsGuid().NotNullable().ForeignKey("FK_CompanyVerifications_CompanyProfileId", "public", "CompanyProfiles", "Id").OnDelete(Rule.None)
                    .WithColumn("Inn").AsString(12).NotNullable()
                    .WithColumn("Kpp").AsString(9).Nullable()
                    .WithColumn("Ogrn").AsString(15).Nullable()
                    .WithColumn("FullName").AsString(500).Nullable()
                    .WithColumn("ShortName").AsString(500).Nullable()
                    .WithColumn("LegalAddress").AsString(1000).Nullable()
                    .WithColumn("ActualAddress").AsString(1000).Nullable()
                    .WithColumn("DirectorName").AsString(500).Nullable()
                    .WithColumn("DirectorPost").AsString(256).Nullable()
                    .WithColumn("RegistrationDate").AsDate().Nullable()
                    .WithColumn("CompanyStatus").AsString(100).Nullable()
                    .WithColumn("ExternalServiceResponse").AsCustom("text").Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_CompanyVerifications_CompanyProfileId")
                    .OnTable("CompanyVerifications")
                    .InSchema("public")
                    .OnColumn("CompanyProfileId");

                Create.Index("IDX_CompanyVerifications_Inn")
                    .OnTable("CompanyVerifications")
                    .InSchema("public")
                    .OnColumn("Inn");
            }

            // Таблица заданий на верификацию компаний
            if (!Schema.Schema("public").Table("CompanyVerificationJobs").Exists())
            {
                Create.Table("CompanyVerificationJobs")
                    .InSchema("public")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("CompanyProfileId").AsGuid().NotNullable().ForeignKey("FK_CompanyVerificationJobs_CompanyProfileId", "public", "CompanyProfiles", "Id").OnDelete(Rule.None)
                    .WithColumn("Inn").AsString(12).NotNullable()
                    .WithColumn("StatusId").AsInt16().NotNullable().ForeignKey("FK_CompanyVerificationJobs_StatusId", "public", "CompanyVerificationJobStatuses", "Id").OnDelete(Rule.None)
                    .WithColumn("StatusComment").AsString(2000).Nullable()
                    .WithColumn("IpAddress").AsString(50).NotNullable()
                    .WithColumn("CompanyVerificationId").AsGuid().Nullable().ForeignKey("FK_CompanyVerificationJobs_CompanyVerificationId", "public", "CompanyVerifications", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("UpdatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("CompletedAt").AsCustom("timestamp with time zone").Nullable();

                Create.Index("IDX_CompanyVerificationJobs_CompanyProfileId")
                    .OnTable("CompanyVerificationJobs")
                    .InSchema("public")
                    .OnColumn("CompanyProfileId");

                Create.Index("IDX_CompanyVerificationJobs_StatusId")
                    .OnTable("CompanyVerificationJobs")
                    .InSchema("public")
                    .OnColumn("StatusId");

                Create.Index("IDX_CompanyVerificationJobs_Inn")
                    .OnTable("CompanyVerificationJobs")
                    .InSchema("public")
                    .OnColumn("Inn");

                Create.Index("IDX_CompanyVerificationJobs_CreatedAt")
                    .OnTable("CompanyVerificationJobs")
                    .InSchema("public")
                    .OnColumn("CreatedAt")
                    .Descending();
            }

            // Таблица истории изменений статусов заданий
            if (!Schema.Schema("public").Table("CompanyVerificationJobHistory").Exists())
            {
                Create.Table("CompanyVerificationJobHistory")
                    .InSchema("public")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("JobId").AsGuid().NotNullable().ForeignKey("FK_CompanyVerificationJobHistory_JobId", "public", "CompanyVerificationJobs", "Id").OnDelete(Rule.Cascade)
                    .WithColumn("StatusId").AsInt16().NotNullable().ForeignKey("FK_CompanyVerificationJobHistory_StatusId", "public", "CompanyVerificationJobStatuses", "Id").OnDelete(Rule.None)
                    .WithColumn("Comment").AsString(2000).Nullable()
                    .WithColumn("CreatedAt").AsCustom("timestamp with time zone").NotNullable().WithDefault(SystemMethods.CurrentDateTime);

                Create.Index("IDX_CompanyVerificationJobHistory_JobId")
                    .OnTable("CompanyVerificationJobHistory")
                    .InSchema("public")
                    .OnColumn("JobId");

                Create.Index("IDX_CompanyVerificationJobHistory_StatusId")
                    .OnTable("CompanyVerificationJobHistory")
                    .InSchema("public")
                    .OnColumn("StatusId");

                // Составной индекс для получения истории Job отсортированной по дате
                Create.Index("IDX_CompanyVerificationJobHistory_JobId_CreatedAt")
                    .OnTable("CompanyVerificationJobHistory")
                    .InSchema("public")
                    .OnColumn("JobId").Ascending()
                    .OnColumn("CreatedAt").Descending();
            }

            // Добавляем поля в WorkerProfiles
            if (Schema.Schema("public").Table("WorkerProfiles").Exists())
            {
                if (!Schema.Schema("public").Table("WorkerProfiles").Column("CompanyProfileId").Exists())
                {
                    Alter.Table("WorkerProfiles")
                        .InSchema("public")
                        .AddColumn("CompanyProfileId").AsGuid().NotNullable()
                        .ForeignKey("FK_WorkerProfiles_CompanyProfileId", "public", "CompanyProfiles", "Id").OnDelete(Rule.None);

                    Create.Index("IDX_WorkerProfiles_CompanyProfileId")
                        .OnTable("WorkerProfiles")
                        .InSchema("public")
                        .OnColumn("CompanyProfileId");
                }
            }

            #endregion
        }

        public override void Down()
        {
            #region Схема public

            // Удаляем таблицу истории
            if (Schema.Schema("public").Table("CompanyVerificationJobHistory").Exists())
            {
                if (Schema.Schema("public").Table("CompanyVerificationJobHistory").Index("IDX_CompanyVerificationJobHistory_JobId_CreatedAt").Exists())
                    Delete.Index("IDX_CompanyVerificationJobHistory_JobId_CreatedAt").OnTable("CompanyVerificationJobHistory").InSchema("public");
                if (Schema.Schema("public").Table("CompanyVerificationJobHistory").Index("IDX_CompanyVerificationJobHistory_StatusId").Exists())
                    Delete.Index("IDX_CompanyVerificationJobHistory_StatusId").OnTable("CompanyVerificationJobHistory").InSchema("public");
                if (Schema.Schema("public").Table("CompanyVerificationJobHistory").Index("IDX_CompanyVerificationJobHistory_JobId").Exists())
                    Delete.Index("IDX_CompanyVerificationJobHistory_JobId").OnTable("CompanyVerificationJobHistory").InSchema("public");

                Delete.Table("CompanyVerificationJobHistory").InSchema("public");
            }

            // Удаляем таблицу заданий
            if (Schema.Schema("public").Table("CompanyVerificationJobs").Exists())
            {
                if (Schema.Schema("public").Table("CompanyVerificationJobs").Index("IDX_CompanyVerificationJobs_CreatedAt").Exists())
                    Delete.Index("IDX_CompanyVerificationJobs_CreatedAt").OnTable("CompanyVerificationJobs").InSchema("public");
                if (Schema.Schema("public").Table("CompanyVerificationJobs").Index("IDX_CompanyVerificationJobs_Inn").Exists())
                    Delete.Index("IDX_CompanyVerificationJobs_Inn").OnTable("CompanyVerificationJobs").InSchema("public");
                if (Schema.Schema("public").Table("CompanyVerificationJobs").Index("IDX_CompanyVerificationJobs_StatusId").Exists())
                    Delete.Index("IDX_CompanyVerificationJobs_StatusId").OnTable("CompanyVerificationJobs").InSchema("public");
                if (Schema.Schema("public").Table("CompanyVerificationJobs").Index("IDX_CompanyVerificationJobs_CompanyProfileId").Exists())
                    Delete.Index("IDX_CompanyVerificationJobs_CompanyProfileId").OnTable("CompanyVerificationJobs").InSchema("public");

                Delete.Table("CompanyVerificationJobs").InSchema("public");
            }

            // Удаляем таблицу верификаций
            if (Schema.Schema("public").Table("CompanyVerifications").Exists())
            {
                if (Schema.Schema("public").Table("CompanyVerifications").Index("IDX_CompanyVerifications_Inn").Exists())
                    Delete.Index("IDX_CompanyVerifications_Inn").OnTable("CompanyVerifications").InSchema("public");
                if (Schema.Schema("public").Table("CompanyVerifications").Index("IDX_CompanyVerifications_CompanyProfileId").Exists())
                    Delete.Index("IDX_CompanyVerifications_CompanyProfileId").OnTable("CompanyVerifications").InSchema("public");

                Delete.Table("CompanyVerifications").InSchema("public");
            }

            // Удаляем таблицу профилей технического надзора
            if (Schema.Schema("public").Table("TechnicalSupervisorProfiles").Exists())
            {
                if (Schema.Schema("public").Table("TechnicalSupervisorProfiles").Index("IDX_TechnicalSupervisorProfiles_IsActive").Exists())
                    Delete.Index("IDX_TechnicalSupervisorProfiles_IsActive").OnTable("TechnicalSupervisorProfiles").InSchema("public");
                if (Schema.Schema("public").Table("TechnicalSupervisorProfiles").Index("IDX_TechnicalSupervisorProfiles_CompanyProfileId").Exists())
                    Delete.Index("IDX_TechnicalSupervisorProfiles_CompanyProfileId").OnTable("TechnicalSupervisorProfiles").InSchema("public");
                if (Schema.Schema("public").Table("TechnicalSupervisorProfiles").Index("IDX_TechnicalSupervisorProfiles_UserId").Exists())
                    Delete.Index("IDX_TechnicalSupervisorProfiles_UserId").OnTable("TechnicalSupervisorProfiles").InSchema("public");

                Delete.Table("TechnicalSupervisorProfiles").InSchema("public");
            }

            // Удаляем поля из WorkerProfiles
            if (Schema.Schema("public").Table("WorkerProfiles").Exists())
            {
                if (Schema.Schema("public").Table("WorkerProfiles").Index("IDX_WorkerProfiles_CompanyProfileId").Exists())
                    Delete.Index("IDX_WorkerProfiles_CompanyProfileId").OnTable("WorkerProfiles").InSchema("public");

                if (Schema.Schema("public").Table("WorkerProfiles").Column("CompanyProfileId").Exists())
                    Delete.Column("CompanyProfileId").FromTable("WorkerProfiles").InSchema("public");
            }

            // Удаляем таблицу профилей компаний
            if (Schema.Schema("public").Table("CompanyProfiles").Exists())
            {
                if (Schema.Schema("public").Table("CompanyProfiles").Index("IDX_CompanyProfiles_IsCompanyVerified").Exists())
                    Delete.Index("IDX_CompanyProfiles_IsCompanyVerified").OnTable("CompanyProfiles").InSchema("public");
                if (Schema.Schema("public").Table("CompanyProfiles").Index("IDX_CompanyProfiles_Inn").Exists())
                    Delete.Index("IDX_CompanyProfiles_Inn").OnTable("CompanyProfiles").InSchema("public");

                Delete.Table("CompanyProfiles").InSchema("public");
            }

            // Удаляем справочник статусов
            if (Schema.Schema("public").Table("CompanyVerificationJobStatuses").Exists())
            {
                Delete.Table("CompanyVerificationJobStatuses").InSchema("public");
            }

            // Удаляем справочник типов компаний
            if (Schema.Schema("public").Table("CompanyTypes").Exists())
            {
                Delete.Table("CompanyTypes").InSchema("public");
            }

            #endregion
        }
    }
}
